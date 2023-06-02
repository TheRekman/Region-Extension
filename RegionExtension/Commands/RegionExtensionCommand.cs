using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegionExtension.Commands.SubCommands;

namespace RegionExtension.Commands
{
    public class RegionExtensionCommand : CommandExtension
    {
        public override string[] Names => new[] { "region", "re"};
        public override string[] Permissions => new[] { TShockAPI.Permissions.manageregion };
        public override string HelpText => "Manages regions.";

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new SetSubCommand(),
            new ClearPointsSubCommand(),
            new DefineSubCommand(),
            new DeleteRegionSubCommand(),
            new GetRegionNameSubCommand(),
            new RenameSubCommand(),
            new RegionListSubCommand(),
            new ResizeSubCommand(),
            new AllowUserSubCommand(),
            new RemoveUserSubCommand(),
            new AllowGroupSubCommand(),
            new RemoveGroupSubCommand(),
            new RegionInfo(),
            new SetProtectSubCommand(),
            new SetZSubCommand(),
            new TeleportToRegionSubCommand(),
            new MoveSubCommand(),
            new SetOwnerSubCommand(),
            new ClearMembersSubCommand(),
            new FastRegionSubCommand(),
            new FastRegionBreakSubCommand(),
            new OwnerList(),
            new LastActiveList()
        }.Concat(base.SubCommands).ToArray();
    }
}
