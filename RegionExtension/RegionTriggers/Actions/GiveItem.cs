using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;
using System.Collections;
using Terraria.ID;

namespace RegionExtension.RegionTriggers.Actions
{
    public class GiveItem : ITriggerAction
    {
        public string Name => "giveitem";
        public string Description => "Gives items to the player.";

        private int _type, _stack, _prefix, _projectile;
        private Function _x, _y, _damage, _usetime;

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "giveitem", "spawnitem", "g", "si" }, "Gives items to the player",
                                                                      new ICommandParam[] {
                                                                          new ItemParam("item", "type of item which will be spawned."),
                                                                          new IntParam("stack", "how many items will be given.", true, 1),
                                                                          new PrefixItemParam("prefix", "item prefix.", true, new Prefix(0)),
                                                                          new FunctionParam("x", "coordinate X of given item. default: px", true, FunctionParam.FunctionParamDefault.PlayerX),
                                                                          new FunctionParam("y", "coordinate Y of given item. default: py", true, FunctionParam.FunctionParamDefault.PlayerY),
                                                                          new FunctionParam("damage", "damage of the item. default: -1", true, "-1"),
                                                                          new FunctionParam("usetime", "usetime of the item. default: -1", true, "-1"),
                                                                          new ProjectileParam("projectile", "used projectile by item. default: null", true, null)
                                                                      },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new GiveItem(s))
        { Permission = Permissions.TriggerMessage };

        private GiveItem(string text)
        {
            var parameters = text.Split(' ');
            _type = int.Parse(parameters[0]);
            _stack = int.Parse(parameters[1]);
            _prefix = int.Parse(parameters[2]);
            _x = new Function(parameters[3]);
            _y = new Function(parameters[4]);
            _damage = new Function(parameters[5]);
            _usetime = new Function(parameters[6]);
            _projectile = int.Parse(parameters[7]);
        }

        public GiveItem(int type, int stack, int prefix, Function x, Function y, Function damage, Function usetime, int projectile)
        {
            _type = type;
            _stack = stack;
            _prefix = prefix;
            _x = x;
            _y = y;
            _damage = damage;
            _usetime = usetime;
            _projectile = projectile;
        }

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            var type = ((Item)param[0].Value).type;
            var stack = (int)param[1].Value;
            var prefix = ((Prefix)param[2].Value).ID;
            var x = (Function)param[3].Value;
            var y = (Function)param[4].Value;
            var damage = (Function)param[5].Value;
            var usetime = (Function)param[6].Value;
            var projectile = param[7].Value == null ? -1 : ((Projectile)param[7].Value).type;
            return new GiveItem(type, stack, prefix, x, y, damage, usetime, projectile);
        }

        public void Execute(TriggerActionArgs args)
        {
            var dmg = _damage.Count(args.Player, args.Region);
            var usetime = _usetime.Count(args.Player, args.Region);
            var useAdvaced = dmg != -1 || usetime != -1 || _projectile != -1;
            var bits = new BitsByte(b2:dmg != -1, b5:usetime != -1, b6:_projectile != -1, b7: _projectile != -1, b8:_projectile != -1);
            var bits2 = new BitsByte(b5:true);
            var id = Item.NewItem(null, (int)Math.Ceiling(_x.Count(args.Player, args.Region) * 16), (int)Math.Ceiling(_y.Count(args.Player, args.Region) * 16), 1, 1, _type, _stack, true, _prefix, true);
            if(dmg != -1)
                Main.item[id].damage = (int)Math.Ceiling(dmg);
            if (usetime != -1)
                Main.item[id].useTime = (int)Math.Ceiling(usetime);
            if (_projectile != -1)
            {
                Main.item[id].shoot = _projectile;
                Main.item[id].useAmmo = AmmoID.None;
                if(Main.item[id].shootSpeed == 0)
                    Main.item[id].shootSpeed = 10;
            }

            NetMessage.SendData(90, -1, -1, null, id);
            if (useAdvaced)
                NetMessage.SendData(88, -1, -1, null, id, bits.value, bits2.value);
        }

        public string GetArgsString() =>
            string.Join(' ', _type, _stack, _prefix, _x.FunctionString, _y.FunctionString, _damage.FunctionString, _usetime.FunctionString, _projectile);
    }
}
