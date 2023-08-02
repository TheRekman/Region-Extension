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

namespace RegionExtension.RegionTriggers.RegionProperties
{
    public class RegionItemBan : IRegionProperty
    {
        public string[] Names => new[] { "itemban", "ib"};
        public string Description => "";
        public string Permission => "regionext.triggers.itemban";
        public ICommandParam[] CommandParams => new[] { new ArrayParam<Item>("items...", "Items which will be banned in region.")};
        public Region[] DefinedRegions =>_itemsBan.Keys.ToArray();

        private Dictionary<Region, List<int>> _itemsBan = new Dictionary<Region, List<int>>();
        private DateTime _lastUpdate = DateTime.Now;
        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            ServerApi.Hooks.GamePostUpdate.Register(plugin, OnPostUpdate);
        }

        private void OnPostUpdate(EventArgs args)
        {
            if (DateTime.Now.AddSeconds(-1) < _lastUpdate)
                return;
            foreach(var plr in TShock.Players.Where(p => p != null && p.Active))
                CheckItemBan(plr);
            _lastUpdate = DateTime.Now;
        }

        public void CheckItemBan(TSPlayer player)
        {
            var reg = player.CurrentRegion;
            if (reg == null || !_itemsBan.ContainsKey(reg))
                return;
            var items = _itemsBan[reg];
            if (items.Contains(player.TPlayer.inventory[player.TPlayer.selectedItem].netID))
            {
                string itemName = player.TPlayer.inventory[player.TPlayer.selectedItem].Name;
                player.Disable($"holding banned item: {itemName}", DisableFlags.None);
                SendCorrectiveMessage(player, itemName);
            }
            if (!Main.ServerSideCharacter || (Main.ServerSideCharacter && player.IsLoggedIn))
            {
                CheckItemInventoryBan(player, player.TPlayer.armor, items);
                CheckItemInventoryBan(player, player.TPlayer.dye, items);
                CheckItemInventoryBan(player, player.TPlayer.miscEquips, items);
                CheckItemInventoryBan(player, player.TPlayer.miscDyes, items);
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
            var itemsToBan = (Item[])commandParams[0].Value;
            if(!_itemsBan.ContainsKey(region))
                _itemsBan.Add(region, new List<int>());
            _itemsBan[region].AddRange(itemsToBan.Select(i => i.type)
                                                 .Where(t => !_itemsBan[region].Contains(t)));
        }

        public void RemoveRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var itemsToBan = (Item[])commandParams[0].Value;
            if (!_itemsBan.ContainsKey(region))
                return;
            _itemsBan[region].RemoveAll(t => itemsToBan.Select(i => i.type)
                                                       .Contains(t));
            if (_itemsBan[region].Count == 0)
                _itemsBan.Remove(region);
        }

        public void SetFromString(Region region, string args)
        {
           var itemsToBan = args.Split(' ').Select(s => int.Parse(s));
           if (!_itemsBan.ContainsKey(region))
                _itemsBan.Add(region, new List<int>());
            _itemsBan[region].AddRange(itemsToBan.Where(t => !_itemsBan[region].Contains(t)));
        }

        public string GetStringArgs(Region region)
        {
            if (!_itemsBan.ContainsKey(region))
                return null;
            return string.Join(" ", _itemsBan[region]);
        }

        public void ClearProperties(Region region) =>
            _itemsBan.Remove(region);
    }
}
