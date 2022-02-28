using SizzleBuildTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildToolLibrary
{
    public partial class SizzleProject
    {

        private Dictionary<string, string> _lastBuildError = new();

        private object _errorMutex = new object();

        public void BuildAllFiles()
        {
            Parallel.ForEach(_rebuildFiles, FileName =>
            {
                TryParseCatchErrors(Path.Combine(ProjectPath, FileName));
            });

            _rebuildFiles.Clear();
        }

        public void BuildSelectedFiles(System.Collections.IList SelectedFiles)
        {
            var result = Parallel.For(0, SelectedFiles.Count, i => {

                string FileAt = SelectedFiles[i] as string;
                TryParseCatchErrors(Path.Combine(ProjectPath, FileAt));
            });

            foreach (string FileAt in SelectedFiles)
            {
                _rebuildFiles.Remove(FileAt);
            }

        }

        public void RebuildAllFiles()
        {
            Parallel.ForEach(_buildFiles, keyValuePair =>
            {
                Parallel.ForEach( keyValuePair.Value, FileName =>
                {
                    string FileAt = keyValuePair.Key as string;
                    FileAt += FileName;

                    TryParseCatchErrors(Path.Combine(ProjectPath, FileAt));

                } );
            });

            _rebuildFiles.Clear();
        }

        void TryParseCatchErrors(string Path)
        {
            try
            {
                FileParser parser = new FileParser();
                parser.Parse(Path);
            }
            catch (Exception ex)
            {
                RegisterError(Path, ex.Message);
                return;
            }

            RegisterError(Path, null);
        }

        void RegisterError(string File, string? Error)
        {
            string RelativePath = Path.GetRelativePath(_projectFilePath, File);
            lock(_errorMutex)
            {
                if (Error != null)
                {
                    if (!FileBuildLastError.ContainsKey(RelativePath))
                    {
                        FileBuildLastError.Add(RelativePath, Error);
                    }
                }
                else
                {
                    if (FileBuildLastError.ContainsKey(RelativePath))
                    {
                        FileBuildLastError.Remove(RelativePath);
                    }    
                }

            }
        }

        public Dictionary<string, string> FileBuildLastError
        {
            get { return _lastBuildError; }
        }

    }
}
