using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SizzleBuildTool
{
    static class Program
    {

        static void Main(string[] args)
        {
            string Path = @"D:\Programare\c++\GameEngine3.0\SizzleBuildTool\SizzleBuildTool\ProjectSimulation\";
            FileManager.Init(Path);

            Console.WriteLine("Press any key!");
            Console.ReadKey();
        }
    }
}
