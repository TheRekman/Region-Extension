using Terraria.ID;
using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockAPI.Localization;
using RegionExtension.RegionTriggers.Conditions;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    public class RegionItemBan : IRegionProperty
    {
        public string[] Names => new[] { "itemban", "ib"};
        public string Description => "Ban items in the region.";
        public string Permission => Permissions.PropertyItem;
        public ICommandParam[] CommandParams => new[] { new ArrayParam<Item>("items...", "Items which will be banned in region.")};
        public Region[] DefinedRegions =>_itemsBan.Keys.ToArray();

        private Dictionary<Region, ConditionDataPair<int>> _itemsBan = new Dictionary<Region, ConditionDataPair<int>>();
        private DateTime _lastUpdate = DateTime.Now;

        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            ServerApi.Hooks.GamePostUpdate.Register(plugin, OnPostUpdate);
        }

        private void OnPostUpdate(EventArgs args)
        {
            if (DateTime.Now.AddSeconds(-1) < _lastUpdate)
                return;
            foreach(var plr in TShock.Players.Where(p => p != null && p.Active && !Plugin.TriggerIgnores[p.Index]))
                CheckItemBan(plr);
            _lastUpdate = DateTime.Now;
        }

        public void CheckItemBan(TSPlayer player)
        {
            var reg = player.CurrentRegion;
            if (reg == null || !_itemsBan.ContainsKey(reg))
                return;
            var items = _itemsBan[reg];
            if(!items.Conditions.CheckConditions(player, reg))
                return;
            if (items.Data.Contains(player.TPlayer.inventory[player.TPlayer.selectedItem].netID))
            {
                string itemName = player.TPlayer.inventory[player.TPlayer.selectedItem].Name;
                player.Disable($"holding banned item: {itemName}", DisableFlags.None);
                SendCorrectiveMessage(player, itemName);
            }
            if (!Main.ServerSideCharacter || (Main.ServerSideCharacter && player.IsLoggedIn))
            {
                CheckItemInventoryBan(player, player.TPlayer.armor, items.Data);
                CheckItemInventoryBan(player, player.TPlayer.dye, items.Data);
                CheckItemInventoryBan(player, player.TPlayer.miscEquips, items.Data);
                CheckItemInventoryBan(player, player.TPlayer.miscDyes, items.Data);
            }
        }

        private void Taint(TSPlayer player)
        {
            player.SetBuff(BuffID.Frozen, 330, true);
            player.SetBuff(BuffID.Stoned, 330, true);
            player.SetBuff(BuffID.Webbed, 330, true);
            player.IsDisabledForBannedWearable = true;
        }

        private void CheckItemInventoryBan(TSPlayer player, IEnumerable<Item> playerItems, IEnumerable<int> bannedId)
        {
            foreach(var item in playerItems)
            {
                if (bannedId.Contains(item.netID))
                {
                    Taint(player);
                    SendCorrectiveMessage(player, item.Name);
                }
            }
        }

        private void SendCorrectiveMessage(TSPlayer player, string itemName)
        {
            player.SendErrorMessage("{0} is banned! Remove it!".SFormat(itemName));
        }

        public void AddRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var itemsToBan = ((Item[])commandParams[0].Value).Select(i => i.type);
            if(!_itemsBan.ContainsKey(region))
                _itemsBan.Add(region, new(new List<IRegionCondition>(), new List<int>()));
            _itemsBan[region].Data.AddRange(itemsToBan);
            _itemsBan[region].Data = _itemsBan[region].Data.GroupBy(x => x).Select(x => x.First()).ToList();
            _itemsBan[region].Data.Sort();
        }

        public void RemoveRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var itemsToBan = (Item[])commandParams[0].Value;
            if (!_itemsBan.ContainsKey(region))
                return;
            _itemsBan[region].Data.RemoveAll(i => itemsToBan.Select(i => i.type).Contains(i));
            if (_itemsBan[region].Data.Count < 1)
                _itemsBan.Remove(region);
        }

        public void SetFromString(Region region, ConditionStringPair args)
        {
           if (!_itemsBan.ContainsKey(region))
                _itemsBan.Add(region, ConditionDataPair<int>.GetFromString(args));
        }

        public ConditionStringPair GetStringArgs(Region region) =>
            _itemsBan[region]?.ConvertToString();

        public void ClearProperties(Region region) =>
            _itemsBan.Remove(region);

        public void AddCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            var itemsToBan = ((Item[])commandParams[0].Value).Select(i => i.type);
            if (!_itemsBan.ContainsKey(region))
                return;
            _itemsBan[region].Conditions = _itemsBan[region].Conditions.Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).Append(condition).ToList();
        }

        public void RemoveCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            var itemsToBan = ((Item[])commandParams[0].Value).Select(i => i.type);
            if (!_itemsBan.ContainsKey(region))
                return;
            _itemsBan[region].Conditions = _itemsBan[region].Conditions.Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).ToList();
        }
    }
}
