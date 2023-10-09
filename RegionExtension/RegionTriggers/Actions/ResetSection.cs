using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace RegionExtension.RegionTriggers.Actions
{
    public class ResetSection : ITriggerAction
    {
        public string Name => "resetsection";
        public string Description => "Resets region section.";

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "resetsection", "rs" }, "ResetSectionDesc",
                                                                      new ICommandParam[] { },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new ResetSection())
        { Permission = Permissions.ResetSectionTrigger };

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args) =>
            new ResetSection();

        public void Execute(TriggerActionArgs args)
        {
            var sX = Netplay.GetSectionX(args.Region.Area.X);
            var sY = Netplay.GetSectionY(args.Region.Area.Y);
            var eX = Netplay.GetSectionX(args.Region.Area.Right);
            var eY = Netplay.GetSectionY(args.Region.Area.Bottom);
            for (int i = sX; i <= eX; i++)
                for (int j = sY; j <= eY; j++)
                    Netplay.Clients[args.Player.Index].TileSections[i, j] = false;
        }

        public string GetArgsString() => null;
    }
}
