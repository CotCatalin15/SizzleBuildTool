using BuildTool.MVVM.View;
using BuildToolLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BuildTool.MVVM.ViewModel
{
    internal class MainWindowsViewModel : ObservableObject
    {
        private RelayCommand _openProject;
        private RelayCommand _createProject;

        private RelayCommand _saveProject;

        public RelayCommand SaveProjectCommand
        {
            get { return _saveProject; }
            set { _saveProject = value; }
        }


        public ObservableCollection<string> FilesToBuild
        {
            get 
            {
                if(_project == null)
                {
                    return null;
                }

                return _project.RebuildFilesNeeded;
            }
            set
            {
                if(_project == null)
                {
                    return;
                }
                OnPropertyChanged();
            }
        }


        private SizzleProject _project;

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


        public MainWindowsViewModel()
        {

            _openProject = new RelayCommand(OpenProjectAction);
            _createProject = new RelayCommand(CreateProjectAction);
            _saveProject = new RelayCommand(SaveProjectAction);
        }

        public void OpenProjectAction(object obj)
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
                        _project.Dispose();
                    }

                    _project = SizzleProject.LoadProject(ProjectFilePath);
                    FilesToBuild = _project.RebuildFilesNeeded;
                }
                catch(Exception ex)
                {

                }
            }

        }

        public void CreateProjectAction(object obj)
        {
            CreateProjectWizzard createWizzard = new CreateProjectWizzard();
            if(createWizzard.ShowDialog() == true)
            {
                CreateProjectWizzardViewModel wizzardModel = createWizzard.DataContext as CreateProjectWizzardViewModel;

                string projectName = wizzardModel.ProjectName;
                string projectPath = wizzardModel.ProjectPath ;


                _project = SizzleProject.CreateDefaultProject(projectPath, projectName);
                FilesToBuild = _project.RebuildFilesNeeded;
            }
        }

        public void SaveProjectAction(object obj)
        {
            if(_project == null)
                return;

            _project.SaveChanges();

        }
    }
}
