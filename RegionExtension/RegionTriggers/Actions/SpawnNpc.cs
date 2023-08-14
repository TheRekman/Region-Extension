using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace RegionExtension.RegionTriggers.Actions
{
    public class SpawnNpc : ITriggerAction
    {
        public string Name => "spawnnpc";
        public string Description => "Spawns npc.";

        private int _type, _count, _health, _strength;
        private Function _x, _y;

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "spawnnpc", "spawnmob", "sn", "sm" }, "SpawnNpcTriggerDesc",
                                                                      new ICommandParam[] {
                                                                          new NpcParam("npc", "type of npc which will be spawned."),
                                                                          new IntParam("count", "how many npc will be spawned. default: 1", true, 1),
                                                                          new FunctionParam("x", "Spawn coordinate by X. default: random in region", true, FunctionParam.FunctionParamDefault.InRegionX),
                                                                          new FunctionParam("y", "Spawn coordinate by Y. default: random in region", true, FunctionParam.FunctionParamDefault.InRegionY),
                                                                          new IntParam("health", "NPC custom max HP. default: Npc", true, -1),
                                                                          new IntParam("strength", "Stats increase of npc. default: gamemode", true, Main.GameMode)
                                                                      },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new SpawnNpc(s))
                                                                      { Permission = Permissions.TriggerSpawnNpc };

        private SpawnNpc(string text)
        {
            var parameters = text.Split(' ');
            _type = int.Parse(parameters[0]);
            _count = int.Parse(parameters[1]);
            _x = new Function(parameters[2]);
            _y = new Function(parameters[3]);
            _health = int.Parse(parameters[4]);
            _strength = int.Parse(parameters[5]);
        }

        public SpawnNpc(int type, int count, int health, int strength, Function x, Function y)
        {
            _type = type;
            _count = count;
            _health = health;
            _strength = strength;
            _x = x;
            _y = y;
        }

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            var npc = (NPC)param[0].Value;
            var health = (int)param[4].Value;
            var type = npc.type;
            return new SpawnNpc(type, (int)param[1].Value, health, (int) param[5].Value, (Function)param[2].Value, (Function)param[3].Value);
        }

        public void Execute(TriggerActionArgs args)
        {
            for(int i = 0; i < _count; i++)
            {
                var stats = new NPCSpawnParams();
                stats.strengthMultiplierOverride = _strength;
                var id = Terraria.NPC.NewNPC(null, (int)Math.Ceiling(_x.Count(args.Player, args.Region) * 16), (int)Math.Ceiling(_y.Count(args.Player, args.Region) * 16), _type);
                Main.npc[id].SetDefaults(Main.npc[id].type, stats);
                if (_health != -1)
                {
                    Main.npc[id].life = _health;
                    Main.npc[id].lifeMax = _health + 1;
                }
                NetMessage.SendData(23, -1, -1, null, id);
                if (_health != -1)
                    Main.npc[id].lifeMax = _health;
            }
        }

        public string GetArgsString() =>
            string.Join(' ', _type.ToString(), _count.ToString(), _x.FunctionString, _y.FunctionString, _health.ToString(), _strength.ToString());
    }
}
