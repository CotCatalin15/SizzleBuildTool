using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SizzleBuildTool
{
    static class XmlCmakeGenerator
    {

        public static void GenerateCmake(string XmlPath)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(XmlPath);

            var attribs = doc["Module"].Attributes;


        }

    }
}
