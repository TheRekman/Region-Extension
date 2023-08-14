using Microsoft.Xna.Framework;
using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    public class ClearPointsSubCommand : SubCommand
    {
        public override string[] Names => new[] { "clear" };
        public override string Description => "ClearPointDesc";

        public override void Execute(CommandArgsExtension args)
        {
            ClearPoints(args);
        }

        private void ClearPoints(CommandArgsExtension args)
        {
            args.Player.TempPoints[0] = Point.Zero;
            args.Player.TempPoints[1] = Point.Zero;
            args.Player.SendSuccessMessage("Points cleared!");
        }
    }
}
