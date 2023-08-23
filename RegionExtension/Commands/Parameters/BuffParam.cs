using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public class BuffParam : CommandParam<Buff>
    {
        public BuffParam(string name, string description, bool optional = false, Buff defaultValue = null) :
            base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var id = -1;
            var buff = new List<int>();
            if (int.TryParse(str, out id) && id >= 0 && Terraria.ID.BuffID.Count > id)
                buff.Add(id); 
            else
                buff = TShock.Utils.GetBuffByName(str);
            if (buff == null || buff.Count == 0)
            {
                args.Player.SendErrorMessage("Failed found buff {0}!".SFormat(str));
                return false;
            }
            if (buff.Count > 1)
            {
                args.Player.SendInfoMessage("Found more than one buff!".SFormat(str));
                args.Player.SendInfoMessage(string.Join(", ", buff.Select(n => "{0}".SFormat(TShock.Utils.GetBuffName(n)))));
                return false;
            }
            _value = new Buff(buff.First());
            return true;
        }
    }
}
