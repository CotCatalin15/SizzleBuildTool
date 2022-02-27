using BuildTool.MVVM.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildTool.MVVM.ViewModel
{
    internal class CreateProjectWizzardViewModel : ObservableObject
    {
        private string _projectName = "";

        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; OnPropertyChanged(); }
        }


        private string _projectPath = "";

        public string ProjectPath
        {
            get { return _projectPath; }
            set { _projectPath = value; OnPropertyChanged(); }
        }

        private RelayCommand _findPathCommand;

        public RelayCommand FindPathCommand
        {
            get { return _findPathCommand; }
            set { _findPathCommand = value; OnPropertyChanged(); }
        }

        private RelayCommand _createProjectCommand;

        public RelayCommand CreateProjectCommand
        {
            get { return _createProjectCommand; }
            set { _createProjectCommand = value; }
        }


        public CreateProjectWizzardViewModel()
        {
            FindPathCommand = new RelayCommand( o =>
            {
                using(FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    if(dialog.ShowDialog() == DialogResult.OK)
                    {
                        ProjectPath = dialog.SelectedPath;
                    }
                }
            });

            _createProjectCommand = new RelayCommand(CreateProjectAction);

        }

        void CreateProjectAction(object o)
        {
            if (_projectName.Length == 0)
            {
                MessageBox.Show("Enter a valid project name!");
                return;
            }

            if (Directory.Exists(_projectPath) == false)
            {
                MessageBox.Show("Entered path is not valid!");
                return;
            }

            if(o == null)
            {
                return;
            }

            CreateProjectWizzard wizzardWindow = o as CreateProjectWizzard;

            if(wizzardWindow == null)
            {
                throw new Exception("Failed to convert command argument to project wizzard!");
            }

            _projectPath = Path.Combine(_projectPath, _projectName);
            Directory.CreateDirectory(_projectPath);

            wizzardWindow.DialogResult = true;
            wizzardWindow.Close();

            return;
        }

    }
}
