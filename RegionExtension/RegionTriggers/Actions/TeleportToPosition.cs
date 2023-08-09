using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.RegionTriggers.Actions
{
    internal class TeleportToPosition : ITriggerAction
    {

        private const int _pushDistance = 5;
        public string Name => "tppos";
        public string Description => "Pushes player from region.";

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "tppos" }, "Teleports player to the position.",
                                                                      new ICommandParam[]
                                                                      {
                                                                          new FunctionParam("x", "x for tp.", false, null),
                                                                          new FunctionParam("y", "y for tp.", false, null)
                                                                      },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new TeleportToPosition(s))
                                                                      { Permission = Permissions.TeleportPositionTrigger };
        Function _x, _y;

        public TeleportToPosition(Function x, Function y)
        {
            _x = x;
            _y = y;
        }

        public TeleportToPosition(string str)
        {
            var p = str.Split(' ');
            _x = new Function(p[0]);
            _y = new Function(p[1]);
        }


        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            var x = (Function)param[0].Value;
            var y = (Function)param[1].Value;
            return new TeleportToPosition(x, y);
        }

        public void Execute(TriggerActionArgs args)
        {
            args.Player.Teleport((float)_x.Count(args.Player, args.Region) * 16, (float)_y.Count(args.Player, args.Region) * 16, 0);
        }

        public string GetArgsString() =>
            string.Join(" ", _x.FunctionString, _y.FunctionString);
    }
}
