using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.RegionTriggers.Actions
{
    internal class TeleportToWarp : ITriggerAction
    {
        public string Name => "warp";
        public string Description => "Teleports player to the warp.";

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "warp" }, "Teleports player to the warp.",
                                                                      new ICommandParam[]
                                                                      {
                                                                          new WarpParam("warp", "warp for tp.", false, null)
                                                                      },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new TeleportToWarp(s))
                                                                      { Permission = Permissions.TriggerWarp };
        string _warpName;

        public TeleportToWarp(string warpname)
        {
            _warpName = warpname;
        }

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            var warpName = ((Warp)param[0].Value).Name;
            return new TeleportToWarp(warpName);
        }

        public void Execute(TriggerActionArgs args)
        {
            var warp = TShock.Warps.Find(_warpName);
            args.Player.Teleport(warp.Position.X * 16, warp.Position.Y * 16, 0);
        }

        public string GetArgsString() =>
            string.Join(" ", _warpName);
    }
}
