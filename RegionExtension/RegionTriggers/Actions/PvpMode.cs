using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL.Terraria;

namespace RegionExtension.RegionTriggers.Actions
{
    public class PvpMode : ITriggerAction
    {
        public string Name => "pvp";
        public string Description => "Changes player pvp mode.";

        private bool _state = true;

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "pvp" }, "PvpTriggerDesc",
                                                                      new ICommandParam[]
                                                                      {
                                                                          new BoolParam("state", "state of pvp mode. Default: true", true, true),
                                                                      },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new PvpMode(s))
        { Permission = Permissions.TriggerKill };

        private PvpMode(string text)
        {
            var parameters = text.Split(' ');
            _state = bool.Parse(parameters[0]);
        }

        private PvpMode(bool state)
        {
            _state = state;
        }

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            var state = (bool)param[0].Value;
            return new PvpMode(state);
        }

        public void Execute(TriggerActionArgs args)
        {
            args.Player.SetPvP(_state);
            args.Player.KillPlayer();
        }

        public string GetArgsString() => null;
    }
}
