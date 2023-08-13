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
        public string Description => "Rewrites npc spawn in the region.";
        public string Permission => Permissions.PropertySpawnRewrite;
        public ICommandParam[] CommandParams => new[] { new ArrayParam<NPCWeightPair>("npcs...", "Npcs which will be spawn in region with weight in format {npc}:{weight}.") };
        public Region[] DefinedRegions => _npcs.Keys.ToArray();

        private Dictionary<Region, ConditionDataPair<NPCWeightPair>> _npcs = new Dictionary<Region, ConditionDataPair<NPCWeightPair>>();
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
            if (pair.Value.Any(p => p.NPCType == e.Type))
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
            npc.type = GetRandomType(_npcs[pair.Value.Item2].Data);
            npc.SetDefaults(npc.type, default(NPCSpawnParams));
            _npcsToIgnoreNetId.Add(new (npc, npc.type));
        }

        private static int GetRandomType(IEnumerable<NPCWeightPair> npcs)
        {
            var weightSum = npcs.Select(n => n.Weight).Sum();
            var randomDouble = _random.NextDouble();
            var finalNum = weightSum * randomDouble;
            var enumerator = npcs.GetEnumerator();
            var tempSum = 0f;
            do
            {
                enumerator.MoveNext();
                var npc = enumerator.Current;
                tempSum += npc.Weight;
            } while (tempSum < finalNum);
            return enumerator.Current.NPCType;
        }

        public void AddRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var itemsToBan = ((NPCWeightPair[])commandParams[0].Value);
            if (!_npcs.ContainsKey(region))
                _npcs.Add(region, new(new List<IRegionCondition>(), new List<NPCWeightPair>()));
            _npcs[region].Data.AddRange(itemsToBan);
            _npcs[region].Data = _npcs[region].Data.GroupBy(x => x).Select(x => x.First()).ToList();
            _npcs[region].Data = _npcs[region].Data.OrderBy(n => n.NPCType).ToList();
        }

        public void RemoveRegionProperties(Region region, ICommandParam[] commandParams)
        {
            var itemsToBan = (NPCWeightPair[])commandParams[0].Value;
            if (!_npcs.ContainsKey(region))
                return;
            _npcs[region].Data.RemoveAll(i => itemsToBan.Select(i => i.NPCType).Any(n => n == i.NPCType));
            if (_npcs[region].Data.Count < 1)
                _npcs.Remove(region);
        }

        public void SetFromString(Region region, ConditionStringPair args)
        {
            if (!_npcs.ContainsKey(region))
                _npcs.Add(region, new ConditionDataPair<NPCWeightPair>(ConditionManager.GetRegionConditionsFromString(args.Conditions).ToList(),
                                                                       args.Args.Split(' ').Select(s => new NPCWeightPair(s)).ToList()));
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
