﻿using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.Conditions
{
    public class Pause : IRegionCondition
    {
        private StringTime _time;
        private DateTime _lastCheck = DateTime.MinValue;
        public static string[] Names { get; } = new[] { "pause", "p" };
        public static string Description { get; } = "pauses trigger in given time after activation.";
        public static ICommandParam[] CommandParam { get; } = new ICommandParam[] { new TimeParam("time", "time after which trigger will be unpaused. default: 1m", true, new StringTime(0, 0, 1, 0)) };
        public static ConditionFormer ConditionFormer { get; } = new ConditionFormer(Names, Description, CommandParam, (cp, rev) => new Pause(cp, rev), (s) => new Pause(s));
        public bool Reversed { get; }

        public Pause(StringTime time, bool reversed)
        {
            _time = time;
            Reversed = reversed;
        }

        public Pause(string s) :
            this(StringTime.FromString(s.Split(' ')[1]), s.StartsWith('!'))
        {
        }

        public Pause(ICommandParam[] commandParam, bool reversed) :
            this((StringTime)commandParam[0].Value, reversed)
        {
        }

        public string[] GetNames() => Names;

        public bool Check(TSPlayer player, Region region)
        {
            var res = _lastCheck + _time < DateTime.Now;
            if (res)
                _lastCheck = DateTime.Now;
            return res;
        }

        public string GetString() =>
            string.Join(' ', "!" + Names[0], _time.ToString());

        public bool IsReversed() =>
            Reversed;
    }
}
