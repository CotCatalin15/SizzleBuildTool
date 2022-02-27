using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildToolLibrary
{
    public partial class SizzleProject : IDisposable
    {
        private string _projectFilePath = "";
        private string _projectName = "";

        private Dictionary<string, List<string> > _buildFiles = new Dictionary<string, List<string>>();

        //Files that need to be rebuilt
        private HashSet<string> _rebuildFiles = new();

        ObservableCollection<string> _rebuldFilesOutList = new();

        FileSystemWatcher _watcher;

        public Dictionary<string, List<string> > BuildFiles
        {
            get { return _buildFiles; }
            set { _buildFiles = value; }
        }

        public ObservableCollection<string> RebuildFilesNeeded { get => _rebuldFilesOutList; }

        public static SizzleProject LoadProject(string FilePath)
        {
            SizzleProject project = SizzleProject.ParseProjectConfig(FilePath);
            return project;
        }

        public void SaveChanges()
        {
            CloseProjectConfig();
        }

        public void Dispose()
        {
            CloseProjectConfig();
        }

        void InitFileWatcher()
        {
            if (_watcher == null)
            {
                _watcher = new FileSystemWatcher(_projectFilePath);
                _watcher.IncludeSubdirectories = true;
                _watcher.EnableRaisingEvents = true;
                _watcher.Filter = "*.h";
                _watcher.Changed += _watcher_Changed;
            }
            else
            {
                _watcher.Path = _projectFilePath;
            }
        }


        private void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string relativeFullPath = Path.GetRelativePath(_projectFilePath, e.FullPath);
            string relaivePath = Path.GetDirectoryName(relativeFullPath);



            List<string> outList;
            if(_buildFiles.TryGetValue(relaivePath, out outList))
            {
                foreach(var file in outList)
                {
                    if(file == e.Name)
                    {
                        if (_rebuildFiles.Add(file))
                        {
                            _rebuldFilesOutList.Add(file);
                        }
                    }
                }
            }

            if (_rebuildFiles.Add(relativeFullPath))
            {
                _rebuldFilesOutList.Add(relativeFullPath);
            }

        }

        public string ProjectPath { get { return _projectFilePath; } set {
                _projectFilePath = value;
                InitFileWatcher();
            } }

        public string ProjectName { get { return _projectName; } set { _projectName = value; } }

    }
}
