using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public class PrefixItemParam : CommandParam<Prefix>
    {

        public PrefixItemParam(string name, string description, bool optional = false, Prefix paramDefault = null)
            : base(name, description, optional, paramDefault)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var value = TShock.Utils.GetPrefixByIdOrName(str).Select(i => new Prefix(i)).ToList();
            if ((value == null || value.Count == 0))
            {
                if(str == "0")
                    value.Add(new Prefix(0));
                else
                {
                    args?.Player.SendErrorMessage("Failed found prefix: {0}".SFormat(str));
                    return false;
                }
            }
            if (value.Count > 1 )
            {
                args.Player.SendInfoMessage("Found more than one prefix!".SFormat(str));
                args.Player.SendInfoMessage(string.Join(", ", value.Select(n => "({0}) {1}".SFormat(n.ID.ToString(), n.Name))));
                return false;
            }
            _value = value.First();
            return true;
        }
    }
}
