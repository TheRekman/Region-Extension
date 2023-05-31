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
        public override string[] Names => new[] { "/regionext", "/re"};
        public override string[] Permissions => new[] { TShockAPI.Permissions.manageregion };
        public override string HelpText => "Provides more commands for region managment";

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new MoveSubCommand(),
            new SetOwnerSubCommand(),
            new ClearMembersSubCommand(),
            new FastRegionSubCommand(),
            new FastRegionBreakSubCommand(),
            new OwnerList(),
            new AllowUserSubCommand(),
            new RemoveUserSubCommand(),
            new AllowGroupSubCommand(),
            new RemoveGroupSubCommand(),
            new DefineSubCommand(),
            new DeleteRegionSubCommand(),
            new SetZSubCommand(),
            new ResizeSubCommand(),
            new RenameSubCommand(),
            new TeleportToRegionSubCommand(),
            new RegionInfo(),
            new ClearPointsSubCommand(),
            new GetRegionNameSubCommand(),
            new RegionListSubCommand()
        }.Concat(base.SubCommands).ToArray();
    }
}
