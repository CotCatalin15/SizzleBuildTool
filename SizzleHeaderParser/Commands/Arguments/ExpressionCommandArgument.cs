using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands.Arguments
{
    //Only equal operator for now
    class ExpressionCommandArgument : ICommandArgument
    {
        string _varName;
        object _value;

        public ExpressionCommandArgument(string VarName, object Value)
        {
            _varName = VarName;
            _value = Value;
        }

        public string VarName => _varName;
        public object Value => _value;

        public CommandArgType Type => CommandArgType.Expression;
    }
}
