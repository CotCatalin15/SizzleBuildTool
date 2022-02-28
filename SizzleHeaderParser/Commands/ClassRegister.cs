using SizzleBuildTool.BuildHelp;
using SizzleBuildTool.Commands.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands
{
    class ClassRegister : IBuildCommand
    {
        public string BuildName => "ClassRegister";

        public bool CanExecute(ICommandArgument[] arguments)
        {
            if(arguments.Length != 1)
                throw new Exception("ClassRegister only accepts one argument(class_name)");

            ValueCommandArgument arg = arguments[0] as ValueCommandArgument;
            if (arg == null)
                throw new Exception("ClassRegister only accepts one argument(class_name)");

            if (arg.Value is string)
                return true;

            throw new Exception("ClassRegister argument must be a string");
        }

        public void Execute(FileParser Parser, ICommandArgument[] arguments)
        {
            //It can execute
            string className = arguments[0].Value as string;

            //Other build tool will do this
            BuildUtil.BuildWriteRegisterClass(className);
        }
    }
}
