using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using IL.Terraria;

namespace RegionExtension.RegionTriggers.Actions
{
    public class PushAction : ITriggerAction
    {
        public string Name => "push";
        public string Description => "Pushes player from region.";

        public static ActionFormer Former { get; } = new ActionFormer("push", "Pushes player from region",
                                                                      new ICommandParam[] { },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new PushAction());
        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args) =>
            new PushAction();

        public void Execute(TriggerActionArgs args)
        {
            var distanceLeft = args.Player.TileX - args.Region.Area.X;
            var distanceRight = Math.Abs(args.Player.TileX - (args.Region.Area.X + args.Region.Area.Width));
            var distanceUp = args.Player.TileY - args.Region.Area.Y;
            var distanceDown = Math.Abs(args.Player.TileY - (args.Region.Area.Y + args.Region.Area.Height));
            var minX = Math.Min(distanceLeft, distanceRight);
            var minY = Math.Min(distanceUp, distanceDown);
            int newX = distanceLeft < distanceRight ? -distanceLeft : distanceRight;
            int newY = distanceUp < distanceDown ? -distanceUp : distanceDown;
            if (minX < minY)
                newY *= (int)Math.Ceiling(args.Region.Area.Height / (float)args.Region.Area.Width);
            else
                newX *= (int)Math.Ceiling(args.Region.Area.Width / (float)args.Region.Area.Height);
            args.Player.Teleport((args.Player.TileX + newX) * 16, (args.Player.TileY + newY) * 16);
        }

        public string GetArgsString() => null;

        public void SetArgsString(string args) { }
    }
}
