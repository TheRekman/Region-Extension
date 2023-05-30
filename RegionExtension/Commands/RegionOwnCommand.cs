using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegionExtension.Commands.SubCommands;

namespace RegionExtension.Commands
{
    public class RegionOwnCommand : CommandExtension
    {
        public override string[] Names => new[] { "/regionown", "/ro" };
        public override string[] Permissions => new[] { "regionext.own" };
        public override string HelpText => "Provides commands for regions in which you owner.";

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new SetOwnerSubCommand(true),
            new ClearMembersSubCommand(true),
            new SelfOwnerList(),
            new AllowUserSubCommand(true),
            new RemoveUserSubCommand(true),
            new RegionInfo(true)
        }.Concat(base.SubCommands).ToArray();
    }
}
