using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public class NpcParam : CommandParam<NPC>
    {
        public NpcParam(string name, string description, bool optional = false, NPC defaultValue = null) :
    base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var npc = TShock.Utils.GetNPCByIdOrName(str);
            if (npc == null || npc.Count == 0)
            {
                args.Player.SendErrorMessage("Failed found item {0}!".SFormat(str));
                return false;
            }
            if (npc.Count > 1)
            {
                args.Player.SendInfoMessage("Found more than one npc!".SFormat(str));
                args.Player.SendInfoMessage(string.Join(", ", npc.Select(n => "({0}) {1}".SFormat(n.type.ToString(), n.FullName))));
                return false;
            }
            _value = npc.First();
            return true;
        }
    }
}
