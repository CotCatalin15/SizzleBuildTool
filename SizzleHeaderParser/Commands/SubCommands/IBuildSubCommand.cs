using SizzleBuildTool.Commands.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SizzleBuildTool.Commands.SubCommands
{
    public interface IBuildSubCommand
    {
        string CommandName { get; }

        bool CanExecute(IBuildCommand Caller, ICommandArgument[] arguments);

        void Execute(FileParser Parser, IBuildCommand Caller, ICommandArgument[] arguments);
    }
}
