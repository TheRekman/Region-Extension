using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Commands
{
    public class RegionExtensionCommand : CommandExtension
    {
        public RegionExtensionCommand()
        {

        }

        public override string[] Names => new[] { "/regionext", "/re"};

        public override string[] Permissions => new[] { TShockAPI.Permissions.manageregion };

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new RenameSubCommand(),
            new MoveSubCommand()
        };
    }
}
