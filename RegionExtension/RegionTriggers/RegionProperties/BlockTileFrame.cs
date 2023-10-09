using Terraria;
using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    internal class BlockTileFrame : IRegionProperty
    {
        public string[] Names => new[] { "blocktileframe", "btf" };
        public string Description => "BlockTileFramePropDesc";
        public string Permission => Permissions.PropertyBlockTileFrame;
        public ICommandParam[] CommandParams => new ICommandParam[0];
        public Region[] DefinedRegions => _regions.Keys.ToArray();

        private Dictionary<Region, List<IRegionCondition>> _regions = new Dictionary<Region, List<IRegionCondition>>();

        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            On.Terraria.WorldGen.TileFrame += OnTileFrame;
        }

        private void OnTileFrame(On.Terraria.WorldGen.orig_TileFrame orig, int i, int j, bool resetFrame, bool noBreak)
        {
            if (_regions.Any(r => r.Key.InArea(i, j)))
                return;
            orig.Invoke(i, j, resetFrame, noBreak);
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

        public void Dispose(Plugin plugin)
        {
            On.Terraria.WorldGen.TileFrame -= OnTileFrame;
        }
    }
}
