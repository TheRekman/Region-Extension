using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.Conditions
{
    internal class Exact : IRegionCondition
    {
        private int _count;
        public static string[] Names { get; } = new[] { "exact", "e" };
        public static string Description { get; } = "ExactCondDesc";
        public static ICommandParam[] CommandParam { get; } = new ICommandParam[] { new IntParam("count", "count") };
        public static ConditionFormer ConditionFormer { get; } = new ConditionFormer(Names, Description, CommandParam, (cp, rev) => new Exact(cp, rev), (s) => new Exact(s));
        public bool Reversed { get; }

        public Exact(int count, bool reversed)
        {
            _count = count;
            Reversed = reversed;
        }

        public Exact(string s) :
            this(int.Parse(s.Split(' ')[1]), s.StartsWith('!'))
        {
        }

        public Exact(ICommandParam[] commandParam, bool reversed) :
            this((int)commandParam[0].Value, reversed)
        {
        }

        public string[] GetNames() => Names;

        public bool Check(TSPlayer player, Region region)
        {
            var count = TShock.Players.Where(p => p != null && p.Active && p.CurrentRegion == region).Count();
            return (count < _count) ^ Reversed;
        }

        public string GetString() =>
            string.Join(' ', (Reversed ? "!" : "") + Names[0], _count.ToString());

        public bool IsReversed() =>
            Reversed;
    }
}
