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
    internal class Hostile : IRegionCondition
    {
        public static string[] Names { get; } = new[] { "pvp", "hostile" };
        public static string Description { get; } = "HostileCondDesc";
        public static ICommandParam[] CommandParam { get; } = new ICommandParam[0];
        public bool Reversed { get; }
        public static ConditionFormer ConditionFormer { get; } = new ConditionFormer(Names, Description, CommandParam, (cp, rev) => new Hostile(cp, rev), (s) => new Hostile(s));

        public Hostile(bool reversed)
        {
            Reversed = reversed;
        }

        public Hostile(string s) :
           this(s.StartsWith('!'))
        {
        }

        public Hostile(ICommandParam[] commandParam, bool reversed) :
            this(reversed)
        {
        }

        public string[] GetNames() => Names;

        public bool Check(TSPlayer player, Region region, Trigger trigger = null) =>
            (player.TPlayer.hostile) ^ Reversed;

        public string GetString() =>
            string.Join(' ', (Reversed ? "!" : "") + Names[0]);

        public bool IsReversed() =>
            Reversed;
    }
}
