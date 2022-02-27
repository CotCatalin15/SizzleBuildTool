using SizzleBuildTool.Commands.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands.SubCommands
{
    //Variable reflexion sub-command
    class Reflect : IBuildSubCommand
    {
        public string CommandName => "Reflect";

        public bool CanExecute(IBuildCommand Caller, ICommandArgument[] arguments)
        {
            //No specific list or arguments needed
            return true;
        }

        public void Execute(IBuildCommand Caller, ICommandArgument[] arguments)
        {
            string VariableLine = FileParser.ReadNextLineSubCmd(this);
            string[] varType = VariableLine.Split(new char[] { '\t', '\n', '\r', ' ', ';'}, StringSplitOptions.RemoveEmptyEntries);
            if(varType.Length != 2)
            {
                Console.WriteLine("Variable must be of like this: type name; ex: int a;");
                return;
            }

            ParseClass ParseCmd = Caller as ParseClass;

            if (ParseCmd == null)
                throw new Exception("Only ParseClass can call this");

            List<string> Properties = new List<string>();
            List<KeyValuePair<string, object>> Expression = new List<KeyValuePair<string, object>>();

            if (arguments != null)
            {
                foreach (var arg in arguments)
                {
                    switch (arg.Type)
                    {
                        case CommandArgType.Value:
                            string val = arg.Value as string;
                            if (val == null)
                                throw new Exception("Arguments can only be strings or expressions!");
                            Properties.Add(val);
                            break;
                        case CommandArgType.Expression:
                            ExpressionCommandArgument exprCommand = arg as ExpressionCommandArgument;
                            Expression.Add(new KeyValuePair<string, object>(exprCommand.VarName, exprCommand.Value));
                            break;
                    }
                }
            }

            FieldEntry entry = new FieldEntry(varType[0], varType[1], Properties, Expression);
            ParseCmd.AddField(entry);
        }
    }
}
