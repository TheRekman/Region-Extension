using RegionExtension.Commands.Parameters;
using RegionExtension.Database.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.Conditions
{
    public class Delay : IRegionCondition
    {
        public static SortedList<DateTime, DelayInfo> DelayedTriggers { get; set; } = new SortedList<DateTime, DelayInfo>();
        private StringTime _time;
        private string _flag;
        public static string[] Names { get; } = new[] { "delay", "d" };
        public static string Description { get; } = "DelayCondDesc";
        public static ICommandParam[] CommandParam { get; } = new ICommandParam[] { 
            new TimeParam("time", "time after which trigger will be will be activated. default: 5s", true, new StringTime(0, 0, 0, 5)),
            new DelayFlagParam("flag", "condition of delay. -f, -i, -a. Default: -f", true, "-f")
        };
        public static ConditionFormer ConditionFormer { get; } = new ConditionFormer(Names, Description, CommandParam, (cp, rev) => new Delay(cp, rev), (s) => new Delay(s));
        public bool Reversed { get; }

        public Delay(StringTime time, string flag, bool reversed)
        {
            _time = time;
            _flag = flag;
            Reversed = reversed;
        }

        public Delay(string s) :
            this(StringTime.FromString(s.Split(' ')[1]), s.Split(' ')[2], s.StartsWith('!'))
        {
        }

        public Delay(ICommandParam[] commandParam, bool reversed) :
            this((StringTime)commandParam[0].Value, (string)commandParam[1].Value, reversed)
        {
        }

        public string[] GetNames() => Names;

        public bool Check(TSPlayer player, Region region, Trigger trigger = null)
        {
            DelayManager.RegisterDelay(new DelayInfo(false, player, region, trigger), _flag, DateTime.Now + _time);
            return false;
        }

        public string GetString() =>
            string.Join(' ', (Reversed ? "!" : "") + Names[0], _time.ToString(), _flag);

        public bool IsReversed() =>
            Reversed;
    }
    public class DelayInfo
    {
        public DelayInfo(bool recheckPlayer, TSPlayer player, Region region, Trigger trigger)
        {
            RecheckPlayer = recheckPlayer;
            Player = player;
            Region = region;
            Trigger = trigger;
        }

        public bool RecheckPlayer { get; private set; }
        public TSPlayer Player { get; private set; }
        public Region Region { get; private set; }
        public Trigger Trigger { get; private set; }
    }
}
