using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands.Arguments
{
    enum CommandArgType
    {
        Value,
        Expression
    }

    interface ICommandArgument
    {
        public CommandArgType Type { get; }
        public object Value { get; }
    }
}
