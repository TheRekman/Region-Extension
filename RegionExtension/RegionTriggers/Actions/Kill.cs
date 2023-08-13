using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.RegionTriggers.Actions
{
    internal class Kill : ITriggerAction
    {
        public string Name => "kill";
        public string Description => "Kills player.";

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "kill", "k" }, "Kills player.",
                                                                      new ICommandParam[] { },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new Kill())
                                                                      { Permission = Permissions.TriggerKill };

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args) =>
            new Kill();

        public void Execute(TriggerActionArgs args)
        {
            args.Player.KillPlayer();
        }

        public string GetArgsString() => null;
    }
}
