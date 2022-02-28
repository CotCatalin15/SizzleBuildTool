using BuildTool.MVVM.View;
using BuildToolLibrary;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace BuildTool.MVVM.ViewModel
{
    internal class MainWindowsViewModel : ObservableObject
    {
        private RelayCommand _openProject;
        private RelayCommand _createProject;
        private RelayCommand _saveProject;

        private RelayCommand _buildSelectedFiles;
        private RelayCommand _buildAllFilesCommand;
        private RelayCommand _rebuildAllFiles;

        private RelayCommand _buildFileManager;

        private string _currentProjectName = "EMPTY";

        private ObservableCollection<string> _buildFiles = new();

        private ObservableCollection<string> _buildFilesError = new();

        public ObservableCollection<string> ErrorSourceItems
        {
            get { return _buildFilesError; }
            set { _buildFilesError = value; }
        }


        private void InitFileSystem()
        {
            if (_watcher == null)
            {
                _watcher = new FileSystemWatcher(_project.ProjectPath);
                _watcher.IncludeSubdirectories = true;
                _watcher.EnableRaisingEvents = true;
                _watcher.Filter = "*.h";
                _watcher.Changed += _watcher_Changed; ;
            }
            else
            {
                _watcher.Path = _project.ProjectPath;
            }

        }

        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.Name.Contains(".generated"))
            {
                return;
            }

            string relativePath = _project.NotifyChangedFile(e.FullPath, e.Name);
            if (relativePath != null)
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    if(!FilesToBuild.Contains(relativePath))
                        FilesToBuild.Add(relativePath);
                });
            }
        }


        private FileSystemWatcher _watcher;

        private SizzleProject _project;


        public MainWindowsViewModel()
        {
            _openProject = new RelayCommand(OpenProjectAction);
            _createProject = new RelayCommand(CreateProjectAction);
            _saveProject = new RelayCommand(SaveProjectAction);

            _buildAllFilesCommand = new RelayCommand(o => {
                if (_project != null)
                {
                    _project.BuildAllFiles();
                    CleanAfterBuild(null);
                }
            });

            _buildSelectedFiles = new RelayCommand(o =>
            {
                ListView view = o as ListView;
                if (_project != null)
                {
                    var list = new List<string>();
                    foreach (string file in view.SelectedItems)
                    {
                        list.Add(file);
                    }

                   _project.BuildSelectedFiles(list);
                    CleanAfterBuild(list);
                }
            });

            _rebuildAllFiles = new RelayCommand(o =>
            {
                if(_project != null)
                {
                    _project.RebuildAllFiles();
                    CleanAfterBuild(null);
                }
            });

            _buildFileManager = new RelayCommand(o => {

                if (_project == null)
                    return;

                BuildFilesWizzardView view = new();

                if (view.DataContext is BuildFilesWizzardViewModel wizzard)
                {
                    wizzard.Project = _project;
                }
                else
                    return;

                view.ShowDialog();

                
                foreach(var file in wizzard.BuildFilesRemoved)
                {
                    _buildFiles.Remove(file);
                }
            });
        }

      
        private void CleanAfterBuild(System.Collections.IList SelectedItems)
        {
            if(SelectedItems == null)
            {
                //Clear the list

                FilesToBuild.Clear();
                FilesToBuild = new ObservableCollection<string>();
                
            }
            else
            {
                foreach (string FileName in SelectedItems)
                {
                    FilesToBuild.Remove(FileName);
                }
            }

            _buildFilesError.Clear();
            foreach (var fileErrorPair in Project.FileBuildLastError)
            {
                _buildFilesError.Add($"File: {fileErrorPair.Key}  Message: {fileErrorPair.Value}");
                if(_buildFiles.Contains(fileErrorPair.Key) == false)
                    _buildFiles.Add(fileErrorPair.Key);
            }

        }

        public void OnWindowClose()
        {
            if(_project != null)
            {
                _project.Dispose();
            }
        }

        private void OpenProjectAction(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "SizzleProject|*.szproj";
            dialog.InitialDirectory = Directory.GetCurrentDirectory();
            dialog.Multiselect = false;

            if(dialog.ShowDialog() == true)
            {
                string ProjectFilePath = dialog.FileName;

                try
                {
                    if(_project != null)
                    {
                        Project.Dispose();
                    }

                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        FilesToBuild.Clear();
                        FilesToBuild = new();
                    });
                    Project = SizzleProject.LoadProject(ProjectFilePath);
                    foreach(var file in Project.RebuldFiles)
                    {
                        FilesToBuild.Add(file);
                    }
                }
                catch(Exception ex)
                {

                }
            }

        }

        private void CreateProjectAction(object obj)
        {
            CreateProjectWizzard createWizzard = new CreateProjectWizzard();
            if(createWizzard.ShowDialog() == true)
            {
                CreateProjectWizzardViewModel wizzardModel = createWizzard.DataContext as CreateProjectWizzardViewModel;

                string projectName = wizzardModel.ProjectName;
                string projectPath = wizzardModel.ProjectPath ;

                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    FilesToBuild.Clear();
                }); 
                Project = SizzleProject.CreateDefaultProject(projectPath, projectName);
                //Save the project practiaclly
            }
        }

        private void SaveProjectAction(object obj)
        {
            if(Project == null)
                return;

            Project.SaveChanges();

        }

        public SizzleProject Project
        {
            get { return _project; }
            set
            {
                _project = value;


                if (_project != null)
                {
                    _project.CloseProjectConfig();
                    InitFileSystem();
                    CurrentProjectName = _project.ProjectName;
                }
            }
        }

        public RelayCommand OpenProject
        {
            get { return _openProject; }
            set { _openProject = value; OnPropertyChanged(); }
        }

        public RelayCommand CreateProject
        {
            get { return _createProject; }
            set { _createProject = value; OnPropertyChanged(); }
        }

        public RelayCommand SaveProjectCommand
        {
            get { return _saveProject; }
            set { _saveProject = value; }
        }


        public RelayCommand BuildSelectedFileCommand
        {
            get { return _buildSelectedFiles; }
            set { _buildSelectedFiles = value; }
        }

        public RelayCommand BuildAllFilesCommand
        {
            get { return _buildAllFilesCommand; }
            set { _buildAllFilesCommand = value; }
        }

        public RelayCommand RebuildAllFilesCommand
        {
            get { return _rebuildAllFiles; }
            set { _rebuildAllFiles = value; }
        }

        public RelayCommand BuildFileManagerCommand
        {
            get { return _buildFileManager; }
            set { _buildFileManager = value; }
        }

        public string CurrentProjectName
        {
            get { return _currentProjectName; }
            set { _currentProjectName = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> FilesToBuild
        {
            get { return _buildFiles; }
            set
            {
                _buildFiles = value;
                OnPropertyChanged();
            }
        }
    }
}
