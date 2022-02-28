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

        private Dictionary<string, HashSet<string> > _buildFiles = new Dictionary<string, HashSet<string>>();

        //Files that need to be rebuilt
        private HashSet<string> _rebuildFiles = new();


        public HashSet<string> RebuldFiles
        {
            get { return _rebuildFiles; }
            set { _rebuildFiles = value; }
        }

        public Dictionary<string, HashSet<string> > BuildFiles
        {
            get { return _buildFiles; }
            set { _buildFiles = value; }
        }


        public static SizzleProject LoadProject(string FilePath)
        {
            SizzleProject project = SizzleProject.ParseProjectConfig(FilePath);
            return project;
        }

        public void AddBuildToolFile(string FilePath, string FileName)
        {
            string relativeFullPath = Path.GetRelativePath(_projectFilePath, FilePath);
            string relaivePath = Path.GetDirectoryName(relativeFullPath);

            if(_buildFiles.ContainsKey(relaivePath))
            {
                _buildFiles[relaivePath].Add(FileName);
                return;
            }

            _buildFiles.Add(relaivePath, new HashSet<string>());
            _buildFiles[relaivePath].Add(FileName);
        }

        public void RemoveBuildToolFile(string FilePath, string FileName)
        {

            if (_buildFiles.TryGetValue(FilePath, out HashSet<string> buildFiles))
            {
                buildFiles.Remove(FileName);
                if(_buildFiles.Count == 0)
                {
                    _buildFiles.Remove(FilePath);
                }
            }

            _rebuildFiles.Remove(FilePath);
        }

        public string NotifyChangedFile(string FilePath, string FileName)
        {
            string relativeFullPath = Path.GetRelativePath(_projectFilePath, FilePath);
            string relaivePath = Path.GetDirectoryName(relativeFullPath);



            HashSet<string> outList = null;
            if (_buildFiles.TryGetValue(relaivePath, out outList))
            {
                if(outList.Contains(FileName))
                {
                    _rebuildFiles.Add(relativeFullPath);
                    return relativeFullPath;
                }
                return null;
            }

            return null;
        }

        public void SaveChanges()
        {
            CloseProjectConfig();
        }

        public void Dispose()
        {
            CloseProjectConfig();
        }



        public string ProjectPath { get { return _projectFilePath; } set {
                _projectFilePath = value;
            } }

        public string ProjectName { get { return _projectName; } set { _projectName = value; } }

    }
}
