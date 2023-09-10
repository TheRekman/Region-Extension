using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Actions;
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
using TShockAPI.Hooks;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    internal class MaxSpawnRewrite : IRegionProperty
    {
        public string[] Names => new[] { "maxspawn", "ms" };
        public string Description => "MaxSpawnPropDesc";
        public string Permission => Permissions.PropertyMaxSpawn;
        public ICommandParam[] CommandParams => new ICommandParam[] { new FloatParam("ratio", "change player npc counting.\n Example: if there are 10 NPCs near the player and the ratio is 0.8, the number of NPCs for player will be 8, and more NPCs may spawn nearby" )};
        public Region[] DefinedRegions => _regions.Keys.ToArray();

        private Dictionary<Region, ConditionDataPair<float>> _regions = new Dictionary<Region, ConditionDataPair<float>>();

        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            ServerApi.Hooks.GameUpdate.Register(plugin, OnUpdate);
        }

        private void OnUpdate(EventArgs args)
        {
            foreach (var plr in TShock.Players.Where(p => p != null && p.Active && p.CurrentRegion != null))
            {
                if (!_regions.ContainsKey(plr.CurrentRegion))
                    continue;
                if (_regions[plr.CurrentRegion].Data[0] == 0)
                    plr.TPlayer.nearbyActiveNPCs = int.MaxValue;
                else
                    plr.TPlayer.nearbyActiveNPCs *= _regions[plr.CurrentRegion].Data[0];
            }
        }

        public void AddRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var ratio = (float)commandParams[0].Value;
            if (!_regions.ContainsKey(region))
                _regions.Add(region, new ConditionDataPair<float>(new List<IRegionCondition>(), new List<float> { ratio }));
            else
                _regions[region].Data[0] = ratio;
        }

        public void RemoveRegionProperties(Region region, ICommandParam[] commandParams)
        {
            if (_regions.ContainsKey(region))
                _regions.Remove(region);
        }

        public void SetFromString(Region region, ConditionStringPair args)
        {
            if (!_regions.ContainsKey(region))
                _regions.Add(region, ConditionDataPair<float>.GetFromString(args));
        }

        public ConditionStringPair GetStringArgs(Region region) =>
            _regions[region].ConvertToString();

        public void ClearProperties(Region region) =>
            _regions.Remove(region);

        public void AddCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            if (!_regions.ContainsKey(region))
                return;
            _regions[region].Conditions = _regions[region].Conditions.Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).Append(condition).ToList();
        }

        public void RemoveCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            if (!_regions.ContainsKey(region))
                return;
            _regions[region].Conditions = _regions[region].Conditions.Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).ToList();
        }

        public void Dispose(Plugin plugin)
        {
            ServerApi.Hooks.GameUpdate.Deregister(plugin, OnUpdate);
        }
    }
}
