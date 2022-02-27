using SizzleBuildTool.Commands;
using SizzleBuildTool.Commands.SubCommands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SizzleBuildTool
{
    static class FileParser
    {
        static string _path;
        static private FileSystemWatcher _watcher;

        static StreamReader reader;
        static StreamWriter writer;

        static object _mut = new object();

        static HashSet<string> _files = new HashSet<string>();

        static HashSet<string> _filesToParse;

        public static void Init(string Path)
        {
            _path = Path;

            InitXmlFile();
            _filesToParse = new HashSet<string>();

            _watcher = new FileSystemWatcher(Path);

            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
            _watcher.Filter = "*.*";
            _watcher.Changed += _watcher_Changed;
            _watcher.Renamed += _watcher_Renamed;
        }

        public static void Build()
        {
            Console.WriteLine("Starting to parse files!");
            foreach(var file in _filesToParse)
            {
                Console.WriteLine($"Parsing {file}");
                Parse(file);
            }
            _filesToParse.Clear();
            Console.WriteLine("Finish parsing files!");
        }

        private static void _watcher_Renamed(object sender, RenamedEventArgs e)
        {
            lock (_mut)
            {
                string Name = e.Name;
                if (Name.EndsWith(".xml"))
                {
                    //Re-parse the xml file!
                    InitXmlFile();
                }
                else if(Name.EndsWith(".generated.h"))
                {
                    
                }
                else if (Name.EndsWith(".h"))
                {
                    string RelativePath = Path.GetRelativePath(_path, e.FullPath);
                    RelativePath = RelativePath.Replace("\\", "/");

                    if (_files.Contains(RelativePath))
                    {
                        Console.WriteLine($"Adding header for reparsing: {RelativePath}");
                        _filesToParse.Add(e.FullPath);
                    }
                }


            }
        }

        private static void InitXmlFile()
        {
            _files.Clear();
            string xmlPath = Path.Combine(_path, "ObjectHeaders.xml");
            //Search for an xml file
            if (File.Exists( xmlPath ))
            {
                Console.WriteLine($"Parsing xml {Path.GetRelativePath(_path, xmlPath)}");
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlPath);

                var headers = doc["Classes"]?.ChildNodes;

                if(headers != null)
                {
                    foreach(XmlNode header in headers)
                    {
                        XmlAttributeCollection attributes = header.Attributes;

                        if(attributes.Count == 0)
                        {
                            Console.WriteLine("File name attribute is required!");
                            continue;
                        }

                        object obj = attributes[0].Value;

                        if (obj == null)
                        {
                            Console.WriteLine("Warning: No File=... was presented!");
                            continue;
                        }

                        if (obj is string)
                        {
                            string path = obj as string;

                            if (File.Exists(Path.Combine(_path, path)))
                            {
                                _files.Add(path);
                            }
                            else
                                Console.WriteLine($"File: {Path.Combine(_path, path)} does not exist!");

                        }
                        else
                            Console.WriteLine("File= is not string!");
                    }
                }

            }
        }

        private static void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            
        }

        public static void Parse(string FilePath)
        {
            while (true)
            {
                try
                {
                    reader = new StreamReader(FilePath);
                    writer = new StreamWriter(FilePath.Replace(".h", ".generated.h"));
                }
                catch(Exception)
                {
                    continue;
                }
                break;
            }
            writer.WriteLine("#pragma once");

            while (reader.EndOfStream == false)
            {
                string line = reader.ReadLine();
                //We found a command line
                //Parse it
                if (line.Contains("BUILD_EXECUTE_COMMAND"))
                {
                    CommandParser parser = new CommandParser(line);

                    Type type = Type.GetType("SizzleBuildTool.Commands." + parser.CommandName);

                    if (type == null)
                    {
                        throw new Exception($"Command {parser.CommandName} was not found!");
                    }

                    object obj = Activator.CreateInstance(type);
                    IBuildCommand command = obj as IBuildCommand;
                    if(command.CanExecute(parser.CommandArgs))
                    {
                        command.Execute(parser.CommandArgs);
                    }
                    
                }
            }

            reader.Close();
            writer.Close();
        }

        public static void WriteToOutput(string Content)
        {
            writer.WriteLine(Content);
        }

        public static string ReadNextLine(IBuildCommand Cmd)
        {
            if (Cmd == null)
                throw new Exception("dasdkls");


            if (reader.EndOfStream == false)
            {
                string Line = reader.ReadLine();

                if (Line.Contains("BUILD_EXECUTE_COMMAND"))
                    throw new Exception("A command can only execute a subcommand!");

                if (Line.Contains("BUILD_EXECUTE_SUBCOMMAND"))
                {
                    CommandParser cmdParser = new CommandParser(Line);

                    var cmdType = Type.GetType("SizzleBuildTool.Commands.SubCommands." + cmdParser.CommandName);

                    if (cmdType == null)
                        throw new Exception($"Subcommand: {cmdParser.CommandName} does not exist!");

                    object obj = Activator.CreateInstance(cmdType);
                    IBuildSubCommand cmd = obj as IBuildSubCommand;
                    if (cmd.CanExecute(Cmd, cmdParser.CommandArgs))
                    {
                        cmd.Execute(Cmd, cmdParser.CommandArgs);
                    }

                }
                else
                {
                    return Line;
                }
            }

            return "";
        }

        public static string ReadNextLineSubCmd(IBuildSubCommand SubCmd)
        {
            if (SubCmd == null)
                throw new Exception("Subcommand cannot be empty");

            string Line = reader.ReadLine();

            if (reader.EndOfStream == false)
            {
                if (Line.Contains("BUILD_"))
                    throw new Exception("A subcomman cannot execute a command or a subcommand!");
                else
                    return Line;
            }

            return "";
        }

    }
}
