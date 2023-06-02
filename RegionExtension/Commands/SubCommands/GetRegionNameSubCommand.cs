using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Commands.SubCommands
{
    public class GetRegionNameSubCommand : SubCommand
    {
        public override string[] Names => new[] { "name", "n" };
        public override string Description => "Shows the name of the region at the given point. Additional params -u, -z, -p";

        public override void Execute(CommandArgsExtension args)
        {
            GetPoint(args);
        }

        private void GetPoint(CommandArgsExtension args)
        {
            args.Player.SendInfoMessage("Hit a block to get the name of the region.");
            args.Player.AwaitingName = true;
            args.Player.AwaitingNameParameters = args.Parameters.Skip(1).ToArray();
        }
    }
}
