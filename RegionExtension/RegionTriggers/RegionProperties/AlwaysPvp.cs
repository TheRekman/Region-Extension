using NuGet.Packaging;
using NuGet.Protocol.Plugins;
using Org.BouncyCastle.Ocsp;
using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.IO;
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
    internal class AlwaysPvp : IRegionProperty
    {
        public string[] Names => new[] { "alwayspvp", "ap" };
        public string Description => "AlwaysPvpPropDesc";
        public string Permission => Permissions.PropertyPvp;
        public ICommandParam[] CommandParams => new ICommandParam[0];
        public Region[] DefinedRegions => _regions.Keys.ToArray();

        private Dictionary<Region, List<IRegionCondition>> _regions = new Dictionary<Region, List<IRegionCondition>>();

        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            ServerApi.Hooks.NetGetData.Register(plugin, OnGetData);
            TriggerManager.OnEnter += OnEnter;
            TriggerManager.OnIn += OnIn;
        }

        private void OnIn(TriggerActionArgs obj)
        {
            if (obj.Player.TPlayer.hostile)
                return;
            var reg = obj.Region;
            if (reg == null || !_regions.ContainsKey(reg))
                return;
            if (!_regions[reg].CheckConditions(obj.Player, reg))
                return;
            obj.Player.SetPvP(true);
            obj.Player.SendErrorMessage("You cannot change pvp in this region.");
        }

        private void OnEnter(TriggerActionArgs obj)
        {
            if (obj.Player.TPlayer.hostile)
                return;
            var reg = obj.Region;
            if (reg == null || !_regions.ContainsKey(reg))
                return;
            if (!_regions[reg].CheckConditions(obj.Player, reg))
                return;
            obj.Player.SetPvP(true);
            obj.Player.SendInfoMessage("Your pvp changed due region property.");
        }

        private void OnGetData(GetDataEventArgs args)
        {
            var plr = TShock.Players[args.Msg.whoAmI];
            if (plr == null)
                return;
            var reg = plr.CurrentRegion;
            if (reg == null || !_regions.ContainsKey(reg))
                return;
            if (!_regions[reg].CheckConditions(plr, reg))
                return;
            switch (args.MsgID)
            {
                case PacketTypes.TogglePvp:
                    bool hostile;
                    using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                    {
                        reader.BaseStream.Seek(1, SeekOrigin.Begin);
                        hostile = reader.ReadBoolean();
                    }
                    if (hostile)
                        return;
                    plr.SetPvP(true);
                    plr.SendErrorMessage("You cannot change pvp in this region.");
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
            TriggerManager.OnEnter -= OnEnter;
            TriggerManager.OnIn -= OnIn;
        }
    }
}
