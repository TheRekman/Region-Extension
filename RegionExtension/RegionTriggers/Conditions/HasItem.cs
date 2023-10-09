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
    internal class HasItem : IRegionCondition
    {
        private int _itemId;
        public static string[] Names { get; } = new[] { "hasitem", "hi" };
        public static string Description { get; } = "HasItemCondDesc";
        public static ICommandParam[] CommandParam { get; } = new ICommandParam[] { new ItemParam("item", "used item") };
        public static ConditionFormer ConditionFormer { get; } = new ConditionFormer(Names, Description, CommandParam, (cp, rev) => new HasItem(cp, rev), (s) => new HasItem(s));
        public bool Reversed { get; }

        public HasItem(int itemId, bool reversed)
        {
            _itemId = itemId;
            Reversed = reversed;
        }

        public HasItem(string s) :
            this(int.Parse(s.Split(' ')[1]), s.StartsWith('!'))
        {
        }

        public HasItem(ICommandParam[] commandParam, bool reversed) :
            this((int)commandParam[0].Value, reversed)
        {
        }

        public string[] GetNames() => Names;

        public bool Check(TSPlayer player, Region region, Trigger trigger = null) =>
            (player.TPlayer.inventory.Any(i => i.type == _itemId) ||
            player.TPlayer.armor.Any(i => i.type == _itemId) ||
            player.TPlayer.dye.Any(i => i.type == _itemId) ||
            player.TPlayer.miscEquips.Any(i => i.type == _itemId)) ^ Reversed;

        public string GetString() =>
            string.Join(' ', (Reversed ? "!" : "") + Names[0], _itemId.ToString());

        public bool IsReversed() =>
            Reversed;
    }
}
