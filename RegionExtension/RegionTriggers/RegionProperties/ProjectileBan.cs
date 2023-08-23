using NuGet.Packaging;
using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    internal class ProjectileBan : IRegionProperty
    {
        public string[] Names => new[] { "projban", "pb" };
        public string Description => "ProjBanPropDesc";
        public string Permission => Permissions.PropertyProjectile;
        public ICommandParam[] CommandParams => new[] { new ArrayParam<Projectile>("projs...", "Projectiles which will be banned in region.") };
        public Region[] DefinedRegions => _projBans.Keys.ToArray();

        private Dictionary<Region, ConditionDataPair<int>> _projBans = new Dictionary<Region, ConditionDataPair<int>>();
        private DateTime _lastUpdate = DateTime.Now;

        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            ServerApi.Hooks.NetGetData.Register(plugin, OnGetData);
        }

        private void OnGetData(GetDataEventArgs args)
        {
            switch(args.MsgID)
            {
                case PacketTypes.ProjectileNew:
                    CheckProjBan(TShock.Players[args.Msg.whoAmI], args);
                    break;
            }
        }

        public void CheckProjBan(TSPlayer player, GetDataEventArgs args)
        {

            var reg = player.CurrentRegion;
            if (reg == null || !_projBans.ContainsKey(reg))
                return;
            var items = _projBans[reg];
            if (!items.Conditions.CheckConditions(player, reg))
                return;
            using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
            {
                var id = reader.ReadInt16();
                reader.BaseStream.Seek(2 + 4 + 4 + 1, SeekOrigin.Begin);
                var type = reader.ReadInt16();
                if (items.Data.Contains(type))
                {
                    string itemName = Lang._projectileNameCache[type].Value;
                    player.SendErrorMessage($"Used banned projectile in this region: {itemName}", DisableFlags.None);
                    NetMessage.SendData(29, player.Index, -1, null, id);
                    args.Handled = true;
                }
            }
        }

        public void AddRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var itemsToBan = ((Projectile[])commandParams[0].Value).Select(i => i.type);
            if (!_projBans.ContainsKey(region))
                _projBans.Add(region, new(new List<IRegionCondition>(), new List<int>()));
            _projBans[region].Data.AddRange(itemsToBan);
            _projBans[region].Data = _projBans[region].Data.GroupBy(x => x).Select(x => x.First()).ToList();
            _projBans[region].Data.Sort();
        }

        public void RemoveRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var itemsToBan = (Projectile[])commandParams[0].Value;
            if (!_projBans.ContainsKey(region))
                return;
            _projBans[region].Data.RemoveAll(i => itemsToBan.Select(i => i.type).Contains(i));
            if (_projBans[region].Data.Count < 1)
                _projBans.Remove(region);
        }

        public void SetFromString(Region region, ConditionStringPair args)
        {
            if (!_projBans.ContainsKey(region))
                _projBans.Add(region, ConditionDataPair<int>.GetFromString(args));
        }

        public ConditionStringPair GetStringArgs(Region region) =>
            _projBans[region]?.ConvertToString();

        public void ClearProperties(Region region) =>
            _projBans.Remove(region);

        public void AddCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            var itemsToBan = ((Projectile[])commandParams[0].Value).Select(i => i.type);
            if (!_projBans.ContainsKey(region))
                return;
            _projBans[region].Conditions = _projBans[region].Conditions.Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).Append(condition).ToList();
        }

        public void RemoveCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            var itemsToBan = ((Projectile[])commandParams[0].Value).Select(i => i.type);
            if (!_projBans.ContainsKey(region))
                return;
            _projBans[region].Conditions = _projBans[region].Conditions.Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).ToList();
        }

        public void Dispose(Plugin plugin)
        {
            ServerApi.Hooks.NetGetData.Deregister(plugin, OnGetData);
        }
    }
}
