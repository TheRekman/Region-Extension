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
    internal class Owner : IRegionCondition
    {
        public static string[] Names { get; } = new[] { "owner", "o" };
        public static string Description { get; } = "OwnerCondDesc";
        public static ICommandParam[] CommandParam { get; } = new ICommandParam[0];
        public bool Reversed { get; }
        public static ConditionFormer ConditionFormer { get; } = new ConditionFormer(Names, Description, CommandParam, (cp, rev) => new Owner(cp, rev), (s) => new Owner(s));

        public Owner(bool reversed)
        {
            Reversed = reversed;
        }

        public Owner(string s) :
           this(s.StartsWith('!'))
        {
        }

        public Owner(ICommandParam[] commandParam, bool reversed) :
            this(reversed)
        {
        }

        public string[] GetNames() => Names;

        public bool Check(TSPlayer player, Region region) =>
            player.Account != null && ((region.Owner == player.Account.Name) ^ Reversed);

        public string GetString() =>
            string.Join(' ', (Reversed ? "!" : "") + Names[0]);

        public bool IsReversed() =>
            Reversed;
    }
}
