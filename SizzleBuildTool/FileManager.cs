using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool
{
    public delegate void FileAdded(string Path, string FileName);
    public delegate void FileRemoved(string Path, string FileName);
    public delegate void FileRenamed(string Path, string OldName, string FileName);
    public delegate void FileChanged(string Path, string FileName);


    static class FileManager
    {
        enum FileExtension
        {
            Folder,
            Xml,
            Header,
            Source,
            CPP = Header | Source
        }

        private static string _path;
        private static FileSystemWatcher _watcher;
            
        private static XmlManager _xmlManager;

        public static void Init(string Path)
        {
            _path = Path;
            _xmlManager = new XmlManager();



            _watcher = new FileSystemWatcher(Path);

            _watcher.Created += _watcher_Created;
            _watcher.Renamed += _watcher_Renamed;


        }

        private static FileExtension GetFileExtension(string File)
        {
            if (File.Contains('.'))
            {
                if (File.Contains(".xml"))
                    return FileExtension.Xml;

                if (File.Contains(".h"))
                    return FileExtension.Header;
                if (File.Contains(".cpp"))
                    return FileExtension.Source;

            }
            else
                return FileExtension.Folder;

            return FileExtension.Xml;
        }

        private static void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            FileExtension extesion = GetFileExtension(e.Name);
            switch (extesion)
            {
                case FileExtension.Xml:
                    if (XmlFileAdded != null)
                        XmlFileRenamed(Path.GetRelativePath(_path, e.FullPath), e.OldName, e.Name);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        

        private static void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            FileExtension extesion = GetFileExtension(e.Name);
            switch(extesion)
            {
                case FileExtension.Xml:
                    if(XmlFileAdded != null)
                        XmlFileAdded(Path.GetRelativePath(_path, e.FullPath) , e.Name);
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        public static string FullPath => _path;

        public static event FileAdded? XmlFileAdded;   
        public static event FileRemoved? XmlFileRemoved;   
        public static event FileRenamed? XmlFileRenamed;
        public static event FileChanged? XmlFileChanged;   

    }
}
