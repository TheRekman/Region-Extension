using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    internal class ClearItems : IRegionProperty
    {
        public string[] Names => new[] { "clearitems", "ci" };
        public string Description => "ClearItemsPropDesc";
        public string Permission => Permissions.PropertyBanHostile;
        public ICommandParam[] CommandParams => new ICommandParam[0];
        public Region[] DefinedRegions => _regions.Keys.ToArray();

        private Dictionary<Region, List<IRegionCondition>> _regions = new Dictionary<Region, List<IRegionCondition>>();
        private DateTime _lastUpdate;

        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            ServerApi.Hooks.GameUpdate.Register(plugin, OnUpdate);
        }

        private void OnUpdate(EventArgs args)
        {
            if (DateTime.Now.AddSeconds(-2) < _lastUpdate)
                return;
            Task.Run(() =>
            {
                foreach (var item in Terraria.Main.item.Where(i => i != null && i.active && _regions.Keys.Any(r => r.InArea((int)Math.Floor(i.position.X / 16), (int)Math.Floor(i.position.Y / 16)))))
                {

                    item.active = false;
                    NetMessage.SendData((int)PacketTypes.UpdateItemDrop, number:item.whoAmI);
                }    
            });
            _lastUpdate = DateTime.Now;
        }

        public void AddRegionProperties(Region region, ICommandParam[] commandParams)
        {
            if (!_regions.ContainsKey(region))
                _regions.Add(region, new List<IRegionCondition>());
        }

        public void RemoveRegionProperties(Region region, ICommandParam[] commandParams)
        {
            if (_regions.ContainsKey(region))
                _regions.Remove(region);
        }

        public void SetFromString(Region region, ConditionStringPair args)
        {
            if (!_regions.ContainsKey(region))
                _regions.Add(region, ConditionDataPair<int>.GetFromString(args).Conditions);
        }

        public ConditionStringPair GetStringArgs(Region region) =>
            new ConditionDataPair<int>(_regions[region], new List<int> { 1 }).ConvertToString();

        public void ClearProperties(Region region) =>
            _regions.Remove(region);

        public void AddCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            if (!_regions.ContainsKey(region))
                return;
            _regions[region] = _regions[region].Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).Append(condition).ToList();
        }

        public void RemoveCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            if (!_regions.ContainsKey(region))
                return;
            _regions[region] = _regions[region].Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).ToList();
        }
    }
}
