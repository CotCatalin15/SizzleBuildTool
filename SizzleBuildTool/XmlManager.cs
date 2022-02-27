using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool
{
    class XmlManager
    {
        public XmlManager()
        {
            FileManager.XmlFileAdded += FileManager_XmlFileAdded;
            FileManager.XmlFileRenamed += FileManager_XmlFileRenamed;
        }

        private void CheckXmlPath(string XmlPath)
        {
            if (XmlPath.Contains('\\') || XmlPath.Contains('/'))
                throw new Exception("Xml can only be in folder 1 away");
        }

        private void FileManager_XmlFileRenamed(string Path, string OldName, string FileName)
        {
            CheckXmlPath(Path);

            if (OldName.Contains(".xml"))
            {

            }
            else
            {

            }
        }

        private void FileManager_XmlFileAdded(string Path, string FileName)
        {
            CheckXmlPath(Path);
        }
    }
}
