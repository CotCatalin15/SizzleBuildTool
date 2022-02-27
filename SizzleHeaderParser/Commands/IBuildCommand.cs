using SizzleBuildTool.Commands.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands
{
    interface IBuildCommand
    {
        string BuildName { get; }

        bool CanExecute(ICommandArgument[] arguments);

        void Execute(ICommandArgument[] arguments);

    }
}
