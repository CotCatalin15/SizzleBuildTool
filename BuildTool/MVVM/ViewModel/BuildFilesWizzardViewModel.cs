using BuildToolLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace BuildTool.MVVM.ViewModel
{
    internal class BuildFilesWizzardViewModel
    {

        private ObservableCollection<string> _buildItems = new();

        private RelayCommand _addFileCommand;
        private RelayCommand _removeFileCommand;

        private HashSet<string> _filesToRemove = new();

        public HashSet<string> BuildFilesRemoved
        {
            get { return _filesToRemove; }
        }


        SizzleProject _project;

        public SizzleProject Project { get { return _project; } set 
            { 
                _project = value;

                foreach (var files in _project.BuildFiles)
                {
                    foreach (string file in files.Value)
                    {
                        string FilePath = files.Key + file;
                        _buildItems.Add(FilePath);
                    }
                }

                
            } }

        public BuildFilesWizzardViewModel()
        {
            _addFileCommand = new RelayCommand(AddFileAction);

            _removeFileCommand = new RelayCommand((o) =>
            {
                var view = o as System.Windows.Controls.ListView;

                if(o == null)
                {
                    return;
                }

                List<string> fileNames = new();

                foreach (string FileName in view.SelectedItems)
                {
                    fileNames.Add(FileName);
                    BuildFilesRemoved.Add(FileName);
                }

                foreach (string FileName in fileNames)
                {
                    _project.RemoveBuildToolFile(Path.GetDirectoryName(FileName), Path.GetFileName(FileName) );
                    _buildItems.Remove(FileName);
                }

            });

        }

        public ObservableCollection<string> BuildSelectedItems
        {
            get { return _buildItems; }
            set { _buildItems = value; }
        }

        private void AddFileAction(object obj)
        {
            using(OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "HeaderFiles(*.h)|*.h";
                dialog.InitialDirectory = _project.ProjectPath;
                dialog.Multiselect = true;
                if(dialog.ShowDialog() == DialogResult.OK)
                {

                    foreach(string item in dialog.FileNames)
                    {
                        TryAddItem(item);
                    }
                }
            }
        }

        private void TryAddItem(string ItemName)
        {
            string relativePath = Path.GetRelativePath(_project.ProjectPath, ItemName);

            if (!_buildItems.Contains(relativePath))
            {
                _buildItems.Add(relativePath);
                _project.AddBuildToolFile( ItemName, Path.GetFileName(ItemName) );
            }
        }

        public RelayCommand AddFileCommand { get { return _addFileCommand; } }  

        public RelayCommand RemoveFileCommand { get { return _removeFileCommand; } }
    }
}
