using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands.Arguments
{
    class ValueCommandArgument : ICommandArgument
    {
        object _value;
        public ValueCommandArgument(object Value)
        {
            _value = Value;
        }

        public CommandArgType Type => CommandArgType.Value;
        public object Value => _value;
    }
}
