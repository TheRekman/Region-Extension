using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Net;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    internal class BlockDoorToggle : IRegionProperty
    {
        public string[] Names => new[] { "blockdoortoggle", "bdt" };
        public string Description => "BlockDoorTogglePropDesc";
        public string Permission => Permissions.PropertyBlockDoorToggle;
        public ICommandParam[] CommandParams => new ICommandParam[0];
        public Region[] DefinedRegions => _regions.Keys.ToArray();

        private Dictionary<Region, List<IRegionCondition>> _regions = new Dictionary<Region, List<IRegionCondition>>();

        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            ServerApi.Hooks.NetGetData.Register(plugin, OnGetData);
        }

        private void OnGetData(GetDataEventArgs args)
        {
            switch (args.MsgID)
            {
                case PacketTypes.DoorUse:
                    short x, y;
                    using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                    {
                        reader.BaseStream.Seek(1, SeekOrigin.Begin);
                        x = reader.ReadInt16();
                        y = reader.ReadInt16();
                    }
                    if (TShock.Regions.CanBuild(x, y, TShock.Players[args.Msg.whoAmI]))
                        return;
                    var region = TShock.Regions.GetTopRegion(TShock.Regions.InAreaRegion(x, y));
                    if (_regions.ContainsKey(region))
                        args.Handled = true;
                    break;
            }
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
            ServerApi.Hooks.NetGetData.Deregister(plugin, OnGetData);
        }
    }
}
