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
    public class RegionRecheck : IRegionCondition
    {
        public static string[] Names { get; } = new[] { "recheck", "rc" };
        public static string Description { get; } = "RecheckCondDesc";
        public static ICommandParam[] CommandParam { get; } = new ICommandParam[0];
        public bool Reversed { get; }
        public static ConditionFormer ConditionFormer { get; } = new ConditionFormer(Names, Description, CommandParam, (cp, rev) => new RegionRecheck(cp, rev), (s) => new Allowed(s));

        public RegionRecheck(bool reversed)
        {
            Reversed = reversed;
        }

        public RegionRecheck(string s) :
           this(s.StartsWith('!'))
        {
        }

        public RegionRecheck(ICommandParam[] commandParam, bool reversed) :
            this(reversed)
        {
        }

        public string[] GetNames() => Names;

        public bool Check(TSPlayer player, Region region, Trigger trigger = null) =>
            (region.InArea(player.TileX, player.TileY)) ^ Reversed;

        public string GetString() =>
            string.Join(' ', (Reversed ? "!" : "") + Names[0]);

        public bool IsReversed() =>
            Reversed;
    }
}
