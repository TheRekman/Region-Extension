using RegionExtension.Commands.SubCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Commands
{
    public class RegionHistoryCommand : CommandExtension
    {
        public override string[] Names => new[] { "regionhistory", "rh" };
        public override string[] Permissions => new[] { "regionext.history" };
        public override string HelpText => "Manages region history.";

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new UndoSubCommand(),
            new RedoSubCommand(),
            new RestoreSubCommand(),
            new GetStorySubCommand()
        }.Concat(base.SubCommands).ToArray();
    }
}
