using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Terraria;
using Microsoft.Xna.Framework;
using Org.BouncyCastle.Bcpg;

namespace RegionExtension.RegionTriggers.Actions
{
    public class PushAction : ITriggerAction
    {

        private const int _pushDistance = 5;
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
            var plr = args.Player;
            var regArea = args.Region.Area;
            var pushArea = new Rectangle(regArea.X - _pushDistance, regArea.Y - _pushDistance, regArea.Width + _pushDistance * 2, regArea.Height + _pushDistance * 2);
            var localX = plr.TileX - pushArea.X;
            var localY = plr.TileY - pushArea.Y;
            var centerX = pushArea.Width / 2;
            var centerY = pushArea.Height / 2;
            var dX = centerX - localX;
            var dY = centerY - localY;
            int x, y;
            if (pushArea.Width * Math.Abs(dY) < pushArea.Height * Math.Abs(dX))
            {
                if (dX == 0)
                    dX = 1;
                x = Math.Sign(dX) * centerX;
                y = dY * x / dX;
            }
            else
            {
                if (dY == 0)
                    dY = 1;
                y = Math.Sign(dY) * centerY;
                x = dX * y / dY;
            }
            plr.Teleport((plr.TileX - x) * 16, (plr.TileY - y) * 16);
        }

        public string GetArgsString() => null;
    }
}
