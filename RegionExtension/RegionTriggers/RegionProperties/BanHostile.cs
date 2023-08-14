using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using OTAPI;
using RegionExtension.Commands.Parameters;
using RegionExtension.Packet;
using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    public class BanHostile : IRegionProperty
    {
        private NPCInfo[] _ignoreNpc = new NPCInfo[Main.npc.Length];
        private Queue<NPC> _registerNPC = new Queue<NPC>();

        public string[] Names => new[] { "banhostile", "bh" };
        public string Description => "BanHostilePropDesc";
        public string Permission => Permissions.PropertyBanHostile;
        public ICommandParam[] CommandParams => new ICommandParam[0];
        public Region[] DefinedRegions => _regions.Keys.ToArray();

        private Dictionary<Region, List<IRegionCondition>> _regions = new Dictionary<Region, List<IRegionCondition>>();

        public void InitializeEventHandler(TerrariaPlugin plugin)
        {
            ServerApi.Hooks.NpcAIUpdate.Register(plugin, OnAIUpdate);
            ServerApi.Hooks.GameUpdate.Register(plugin, OnUpdate);
            ServerApi.Hooks.GamePostUpdate.Register(plugin, OnPostUpdate);
            ServerApi.Hooks.NpcSpawn.Register(plugin, OnNpcSpawn);
            ServerApi.Hooks.NpcKilled.Register(plugin, OnNpcKilled);
            ServerApi.Hooks.ProjectileAIUpdate.Register(plugin, OnProjectileUpdate);
            OTAPI.Hooks.NPC.Create += OnNPCCreate;
        }

        private void OnProjectileUpdate(ProjectileAiUpdateEventArgs args)
        {
            var proj = args.Projectile;
            if (!proj.hostile)
                return;
            if(_regions.Any(p => p.Key.InArea((int)(proj.position.X / 16), (int)(proj.position.Y / 16))))
                proj.Kill();
        }

        private void OnNPCCreate(object sender, Hooks.NPC.CreateEventArgs e)
        { 
            if (e.Source == null || e.Source.GetType() != typeof(EntitySource_Parent))
                return;
            var entity = ((EntitySource_Parent)e.Source).Entity;
            if (entity.GetType() != typeof(NPC))
                return;
            var npc = (NPC)entity;
            if(!npc.boss && !npc.dontCountMe)
                return;
            e.Npc = new NPC();
            _registerNPC.Enqueue(e.Npc);
        }

        private void OnNpcKilled(NpcKilledEventArgs args)
        {
            _ignoreNpc[args.npc.whoAmI].Ignore = false;
            _ignoreNpc[args.npc.whoAmI].whoAmI = 0;
        }

        private void OnNpcSpawn(NpcSpawnEventArgs args)
        {
            if(_registerNPC.Count > 0)
            {
                while(_registerNPC.Count > 0)
                {
                    var npc = _registerNPC.Dequeue();
                    if (npc.dontCountMe)
                        continue;
                    _ignoreNpc[npc.whoAmI].Ignore = true;
                    _ignoreNpc[npc.whoAmI].whoAmI = npc.whoAmI;
                }
                return;
            }
            _ignoreNpc[args.NpcId].Ignore = Main.npc[args.NpcId].boss;
            _ignoreNpc[args.NpcId].whoAmI = args.NpcId;
        }

        private void OnPostUpdate(EventArgs args)
        {
            foreach (var boss in _ignoreNpc.Where(i => i.Ignore))
            {
                var npc = Main.npc[boss.whoAmI];
                var x = (int)(npc.position.X / 16);
                var y = (int)(npc.position.Y / 16);
                var pairs = _regions.Where(p => p.Key.InArea(x, (int)(boss.LasPosition.Y / 16)));
                var resX = false;
                var resY = false;
                if (pairs.Count() != 0)
                {
                    npc.position.X = boss.LasPosition.X;
                    npc.velocity.X = 0;
                    resX = true;
                }
                pairs = _regions.Where(p => p.Key.InArea((int)(boss.LasPosition.X / 16), y));
                if (pairs.Count() != 0)
                {
                    npc.position.Y = boss.LasPosition.Y;
                    npc.velocity.Y = 0;
                    resY = true;
                }
                if(resX && resY)
                {
                    var _pushDistance = 1;
                    var regArea = _regions.Where(p => p.Key.InArea(x, y)).MaxBy(p => p.Key.Z).Key.Area;
                    var pushArea = new Rectangle(regArea.X - _pushDistance, regArea.Y - _pushDistance, regArea.Width + _pushDistance * 2, regArea.Height + _pushDistance * 2);
                    var localX = x - pushArea.X;
                    var localY = y - pushArea.Y;
                    var centerX = pushArea.Width / 2;
                    var centerY = pushArea.Height / 2;
                    var dX = centerX - localX;
                    var dY = centerY - localY;
                    int nx, ny;
                    if (pushArea.Width * Math.Abs(dY) < pushArea.Height * Math.Abs(dX))
                    {
                        if (dX == 0)
                            dX = 1;
                        nx = Math.Sign(dX) * centerX;
                        ny = dY * nx / dX;
                    }
                    else
                    {
                        if (dY == 0)
                            dY = 1;
                        ny = Math.Sign(dY) * centerY;
                        nx = dX * ny / dY;
                    }
                    npc.position.X = (pushArea.X + (-nx + pushArea.Width / 2)) * 16;
                    npc.position.Y = (pushArea.Y + (-ny + pushArea.Height / 2)) * 16;

                }
                if(resX || resY)
                {
                    //skeletron prime fix
                    if (npc.type == 127 || npc.ai[1] == 32)
                        npc.ai[1] = 0;
                    NetMessage.SendData(23, number: npc.whoAmI);
                }    
            }   
        }

        private void OnUpdate(EventArgs args)
        {
            for (int i = 0; i < _ignoreNpc.Length; i++)
                if (_ignoreNpc[i].Ignore)
                    _ignoreNpc[i].LasPosition = Main.npc[i].position;
        }

        private void OnAIUpdate(NpcAiUpdateEventArgs args)
        {
            if (args.Npc.friendly || args.Npc.dontCountMe || args.Npc.damage == 0 || args.Npc.boss || _ignoreNpc[args.Npc.whoAmI].Ignore)
                return;
            var x = (int)(args.Npc.position.X / 16);
            var y = (int)(args.Npc.position.Y / 16);
            var pairs = _regions.Where(p => p.Key.InArea(x, y));
            if (pairs.Count() == 0)
                return;
            Main.npc[args.Npc.whoAmI].active = false;
            NetMessage.SendData(28, number: args.Npc.whoAmI, number2: -1);
            short pid = 1000;
            for(short i = 0; i < Main.projectile.Length; i++)
                if (!Main.projectile[i].active)
                {
                    pid = i;
                    break;
                }
            if (pid == 1000)
                return;
            Task.Run(() =>
            {
                PacketConstructor.SendPacket(-1, new ProjectileUpdatePacket(pid, args.Npc.position.X + args.Npc.width / 2, args.Npc.position.Y + args.Npc.height / 2, 0, 0, (byte)Main.myPlayer, 12));
                PacketConstructor.SendPacket(-1, new ProjectileDestroyPacket(pid, 255));
            });
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

        struct NPCInfo
        {
            public NPCInfo()
            {
            }

            public NPCInfo(bool ignore, Vector2 lasPosition)
            {
                Ignore = ignore;
                LasPosition = lasPosition;
            }

            public int whoAmI { get; set; } = 0;
            public bool Ignore { get; set; } = true;
            public Vector2 LasPosition { get; set; } = new Vector2();
        }
    }
}
