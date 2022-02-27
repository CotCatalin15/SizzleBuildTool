using SizzleBuildTool.Commands.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands
{
    class CommandParser
    {
        private string _commandName;
        private ICommandArgument[] _commandArgs;

       
        public CommandParser(string Command)
        {
            int pFrom = Command.IndexOf("(") + 1;
            int pTo = Command.LastIndexOf(")");

            string commandArgs = Command.Substring(pFrom, pTo - pFrom);
            string[] commandParams = commandArgs.Split(",");

            if(commandParams.Length < 1)
            {
                throw new Exception("Build command not specified!");
            }

            _commandName = commandParams[0].Trim();
            if(commandParams.Length > 1)
            {
                _commandArgs = new ICommandArgument[commandParams.Length - 1];
                for(int i = 0; i < _commandArgs.Length; ++i)
                {
                    string arg = commandParams[i + 1].Trim();
                    if (arg.Contains('='))
                    {
                        string[] splitArg = arg.Split(new char[]{' ', '='}, StringSplitOptions.RemoveEmptyEntries);

                        _commandArgs[i] = new ExpressionCommandArgument(splitArg[0],
                            ConvertStringToObject(splitArg[1]));
                    }
                    else
                    {
                        _commandArgs[i] = new ValueCommandArgument(ConvertStringToObject(arg));
                    }
                }
            }
        }

        static object ConvertStringToObject(string s)
        {
            int intVal;
            float floatVal;

            if(Int32.TryParse(s, out intVal))
            {
                return intVal;
            }

            if(float.TryParse(s, out floatVal))
            {
                return floatVal;
            }

            return s;
        }

        public string CommandName { get => _commandName; }

        public ICommandArgument[] CommandArgs { get => _commandArgs; }
    }
}
