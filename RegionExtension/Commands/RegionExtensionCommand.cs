using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            new RenameSubCommand(),
            new MoveSubCommand(),
            new SetOwnerSubCommand(),
            new ClearMembersSubCommand(),
            new FastRegionSubCommand(),
            new FastRegionBreakSubCommand(),
            new OwnerList()
        }.Concat(base.SubCommands).ToArray();
    }
}
