using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands.Arguments
{
    public enum CommandArgType
    {
        Value,
        Expression
    }

    public interface ICommandArgument
    {
        public CommandArgType Type { get; }
        public object Value { get; }
    }
}
