using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegionExtension.Commands.SubCommands;
using RegionExtension.Database;

namespace RegionExtension.Commands
{
    public class RegionOwnCommand : CommandExtension
    {
        public override string[] Names => new[] { "regionown", "ro" };
        public override string[] Permissions => new[] { RegionExtension.Permissions.RegionOwnCmd };
        public override string HelpText => "Provides commands for regions in which you owner.";

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new SetOwnerSubCommand(true),
            new ClearMembersSubCommand(true),
            new SelfOwnerList(),
            new AllowUserSubCommand(true),
            new RemoveUserSubCommand(true),
            new RegionInfo(true),
            new SetSubCommand(),
            new DefineRequestSubCommand(),
            new DeleteRegionSubCommand(true),
            new FastRegionRequestSubCommand(),
            new FastRegionBreakSubCommand()
        }.Concat(base.SubCommands).ToArray();
    }
}
