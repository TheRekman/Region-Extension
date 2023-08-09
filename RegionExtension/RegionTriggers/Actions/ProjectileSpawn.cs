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
    public class ProjectileSpawn : ITriggerAction
    {
        public string Name => "spawnproj";
        public string Description => "sp";

        private int _type, _count;
        private Function _knockback, _damage, _x, _y, _xV, _yV;

        private static Random _random = new Random();

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "spawnproj", "sp" }, "Spawns projectile.",
                                                                      new ICommandParam[] {
                                                                          new ProjectileParam("projectile", "type of projectile which will be spawned."),
                                                                          new IntParam("count", "count of projectile. default: 1", true, 1),
                                                                          new FunctionParam("damage", "damage.  default: random", true, FunctionParam.FunctionParamDefault.RandomDouble),
                                                                          new FunctionParam("knockback", "knockback. default: random", true, FunctionParam.FunctionParamDefault.RandomDouble),
                                                                          new FunctionParam("x", "Spawn coordinate by X. default: random in region", true, FunctionParam.FunctionParamDefault.InRegionX),
                                                                          new FunctionParam("y", "Spawn coordinate by Y. default: random in region", true, FunctionParam.FunctionParamDefault.InRegionY),
                                                                          new FunctionParam("speedX", "Speed by X. default: random", true, FunctionParam.FunctionParamDefault.RandomDouble),
                                                                          new FunctionParam("speedY", "Speed by Y. default: random", true, FunctionParam.FunctionParamDefault.RandomDouble)
                                                                      },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new ProjectileSpawn(s))
                                                                      { Permission = Permissions.ProjetileTrigger };

        private ProjectileSpawn(string text)
        {
            var parameters = text.Split(' ');
            _type = int.Parse(parameters[0]);
            _count = int.Parse(parameters[1]);
            _damage = new Function(parameters[2]);
            _knockback = new Function(parameters[3]);
            _x = new Function(parameters[4]);
            _y = new Function(parameters[5]);
            _xV = new Function(parameters[6]);
            _yV = new Function(parameters[7]);
        }

        public ProjectileSpawn(int type, int count, Function knockback, Function damage, Function x, Function y, Function xV, Function yV)
        {
            _type = type;
            _count = count;
            _knockback = knockback;
            _damage = damage;
            _x = x;
            _y = y;
            _xV = xV;
            _yV = yV;
        }

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            var type = ((Projectile)param[0].Value).type;
            var count = (int)param[1].Value;
            var damage = (Function)param[2].Value;
            var knockback = (Function)param[3].Value;
            var x = (Function)param[4].Value;
            var y = (Function)param[5].Value;
            var xV = (Function)param[6].Value;
            var xY = (Function)param[7].Value;
            return new ProjectileSpawn(type, count, knockback, damage, x, y, xV, xY);
        }

        public void Execute(TriggerActionArgs args)
        {
            for(int i = 0; i< _count; i++)
            {
                var proj = Projectile.NewProjectile(null, (float)_x.Count(args.Player, args.Region) * 16, (float)_y.Count(args.Player, args.Region) * 16, (float)_xV.Count(args.Player, args.Region), (float)_yV.Count(args.Player, args.Region), _type, (int)Math.Ceiling(_damage.Count(args.Player, args.Region)), (float)_knockback.Count(args.Player, args.Region));
                Main.projectile[proj].damage = (int)Math.Ceiling(_damage.Count(args.Player, args.Region));
                Main.projectile[proj].knockBack = (float)_knockback.Count(args.Player, args.Region);
                NetMessage.SendData(27, -1, -1, null, proj);
            }
        }

        public string GetArgsString() =>
            string.Join(' ', _type.ToString(), _count.ToString(), _damage.FunctionString, _knockback.FunctionString, _x.FunctionString, _y.FunctionString, _xV.FunctionString, _xV.FunctionString);
    }
}
