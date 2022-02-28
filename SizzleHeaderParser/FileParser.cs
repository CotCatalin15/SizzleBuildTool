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
    public class FileParser
    {
        string _path;

        StreamReader reader;
        StreamWriter writer;


        public FileParser()
        {
        }

 
        public void Parse(string FilePath)
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
            string exMsg = null;
            try
            {
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
                        if (command.CanExecute(parser.CommandArgs))
                        {
                            command.Execute(this, parser.CommandArgs);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                exMsg = ex.Message;
            }
          
            reader.Close();
            writer.Close();

            if(exMsg != null)
                throw new Exception(exMsg);
        }

        public void WriteToOutput(string Content)
        {
            writer.WriteLine(Content);
        }

        public string ReadNextLine(IBuildCommand Cmd)
        {
            if (Cmd == null)
                throw new Exception("dasdkls");


            if (reader.EndOfStream == false)
            {
                string Line = reader.ReadLine();

                PreprocessLine(ref Line);

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
                        cmd.Execute(this, Cmd, cmdParser.CommandArgs);
                    }

                }
                else
                {
                    return Line;
                }
            }

            return null;
        }

        public void PreprocessLine(ref string Line)
        {
            if(Line.Contains("BUILD_REFLECT("))
            {
                Line = Line.Replace("BUILD_REFLECT(", "BUILD_EXECUTE_SUBCOMMAND(Reflect,");
            }
        }

        public string ReadNextLineSubCmd(IBuildSubCommand SubCmd)
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
