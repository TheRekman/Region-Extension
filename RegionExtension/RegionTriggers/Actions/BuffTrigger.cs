using RegionExtension.Commands;
using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.RegionTriggers.Actions
{
    internal class BuffTrigger : ITriggerAction
    {
        public string Name => "buff";
        public string Description => "Buffs player.";

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "buff", "b" }, "BuffTriggerDesc",
                                                                      new ICommandParam[]
                                                                      {
                                                                          new BuffParam("buff", "buff type."),
                                                                          new IntParam("time", "buff time. default: 60", true, 60)
                                                                      },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new BuffTrigger(s))
                                                                      { Permission = Permissions.TriggerBuff };
        int _buff, _time;

        public BuffTrigger(string str)
        {
            var p = str.Split(' ');
            _buff = int.Parse(p[0]);
            _time = int.Parse(p[1]);
        }

        public BuffTrigger(int buff, int time)
        {
            _buff = buff;
            _time = time;
        }

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            var buff = ((Buff)param[0].Value).Id;
            var time = (int)param[1].Value;
            return new BuffTrigger(buff, time);
        }

        public void Execute(TriggerActionArgs args)
        {
            args.Player.SetBuff(_buff, _time * 30);
        }

        public string GetArgsString() =>
            string.Join(" ", _buff, _time);
    }
}
