using NuGet.Packaging;
using OTAPI;
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
    internal class NPCSpawnRewrite : IRegionProperty
    {
        private static Random _random = new Random();
        private static List<(NPC, Region)?> _npcsToChange = new List<(NPC, Region)?>();
        private static List<(NPC, int)?> _npcsToIgnoreNetId = new List<(NPC, int)?>();
        public string[] Names => new[] { "spawnrewrite", "sr" };
        public string Description => "";
        public string Permission => "regionext.triggers.itemban";
        public ICommandParam[] CommandParams => new[] { new ArrayParam<NPC>("npcs...", "Npcs which will be spawn in region.") };
        public Region[] DefinedRegions => _npcs.Keys.ToArray();

        private Dictionary<Region, ConditionDataPair<int>> _npcs = new Dictionary<Region, ConditionDataPair<int>>();
        private DateTime _lastUpdate = DateTime.Now;

        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            OTAPI.Hooks.NPC.Spawn += OnNPCSpawn;
            OTAPI.Hooks.NPC.Create += OnNPCCreate;
            ServerApi.Hooks.NetSendData.Register(plugin, OnSendData);
        }

        private void OnSendData(SendDataEventArgs args)
        {
            if(args.MsgId != PacketTypes.NpcUpdate) return;
            var npc = Main.npc[args.number];
            var pair = _npcsToIgnoreNetId.FirstOrDefault(x => x.Value.Item1 == npc);
            if (pair == null)
                return;
            npc.type = pair.Value.Item2;
            npc.netID = 0;
            npc.SetDefaults(npc.type);
            _npcsToIgnoreNetId.Remove(pair);
        }

        private void OnNPCCreate(object sender, Hooks.NPC.CreateEventArgs e)
        {
            if (e.Source == null || !e.Source.GetType().Equals(NPC.GetSpawnSourceForNaturalSpawn().GetType()))
                return;
            var x = e.X / 16;
            var y = e.Y / 16;
            var pairs = _npcs.Where(p => p.Key.InArea(x, y));
            if (pairs.Count() == 0)
                return;
            var pair = pairs.MaxBy(p => p.Key.Z);
            if (pair.Equals(default(KeyValuePair<Region, ConditionDataPair<int>>)))
                return;
            if (pair.Value.Contains(e.Type))
                return;
            e.Npc = new NPC();
            _npcsToChange.Add(new(e.Npc, pair.Key));
        }

        private void OnNPCSpawn(object sender, Hooks.NPC.SpawnEventArgs e)
        {
            var npc = Main.npc[e.Index];
            var pair = _npcsToChange.FirstOrDefault(x => x.Value.Item1 == npc);
            if (pair == null)
                return;
            _npcsToChange.Remove(pair);
            npc.type = _npcs[pair.Value.Item2].Data[_random.Next(_npcs[pair.Value.Item2].Data.Count())];
            npc.SetDefaults(npc.type, default(NPCSpawnParams));
            _npcsToIgnoreNetId.Add(new (npc, npc.type));
        }

        public void AddRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var itemsToBan = ((NPC[])commandParams[0].Value).Select(i => i.type);
            if (!_npcs.ContainsKey(region))
                _npcs.Add(region, new(new List<IRegionCondition>(), new List<int>()));
            _npcs[region].Data.AddRange(itemsToBan);
            _npcs[region].Data = _npcs[region].Data.GroupBy(x => x).Select(x => x.First()).ToList();
            _npcs[region].Data.Sort();
        }

        public void RemoveRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var itemsToBan = (NPC[])commandParams[0].Value;
            if (!_npcs.ContainsKey(region))
                return;
            _npcs[region].Data.RemoveAll(i => itemsToBan.Select(i => i.type).Contains(i));
            if (_npcs[region].Data.Count < 1)
                _npcs.Remove(region);
        }

        public void SetFromString(Region region, ConditionStringPair args)
        {
            if (!_npcs.ContainsKey(region))
                _npcs.Add(region, ConditionDataPair<int>.GetFromString(args));
        }

        public ConditionStringPair GetStringArgs(Region region) =>
            _npcs[region]?.ConvertToString();

        public void ClearProperties(Region region) =>
            _npcs.Remove(region);

        public void AddCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            var itemsToBan = ((Item[])commandParams[0].Value).Select(i => i.type);
            if (!_npcs.ContainsKey(region))
                return;
            _npcs[region].Conditions = _npcs[region].Conditions.Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).Append(condition).ToList();
        }

        public void RemoveCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition)
        {
            var itemsToBan = ((Item[])commandParams[0].Value).Select(i => i.type);
            if (!_npcs.ContainsKey(region))
                return;
            _npcs[region].Conditions = _npcs[region].Conditions.Where(p => !p.GetNames()[0].Equals(condition.GetNames()[0])).ToList();
        }
    }
}
