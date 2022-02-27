using SizzleBuildTool.Commands;
using System;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace SizzleBuildTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args.Length > 0)
            {
                string[] file = args[0].Split('.');
                path = args[0];
            }
            else
            {
                path = @"D:\Programare\c++\GameEngine3.0\SizzleEngine\SizzleEngine\Runtime\";
                //path = @"D:\Programare\c++\GameEngine3.0\SizzleBuildTool\SizzleHeaderParser\";
            }

            FileParser.Init(path);

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "Build")
                {
                    FileParser.Build();
                }
                else if(input == "ReloadGen")
                {
                   Commands.ParseClass.LoadSGenClass(); 
                }
                else if (input == "q")
                    break;
            }

        }
    }
}
