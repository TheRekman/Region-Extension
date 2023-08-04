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
    public class Allowed : IRegionCondition
    {
        public static string[] Names { get; } = new[] { "allowed", "a" };
        public static string Description { get; } = "if player is allowed in the region.";
        public static ICommandParam[] CommandParam { get; } = new ICommandParam[0];
        public bool Reversed { get; }
        public static ConditionFormer ConditionFormer { get; } = new ConditionFormer(Names, Description, CommandParam, (cp, rev) => new Allowed(cp, rev), (s) => new Allowed(s));

        public Allowed(bool reversed)
        {
            Reversed = reversed;
        }

        public Allowed(string s) :
           this(s.StartsWith('!'))
        {
        }

        public Allowed(ICommandParam[] commandParam, bool reversed) :
            this(reversed)
        {
        }

        public string[] GetNames() => Names;

        public bool Check(TSPlayer player, Region region) =>
            player.Account != null && ((region.AllowedIDs.Contains(p.Account.ID) || region.Owner == player.Account.Name) ^ Reversed);

        public string GetString() =>
            string.Join(' ', Names[0]);

        public bool IsReversed() =>
            Reversed;
    }
}
