using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public class NPCWeightPairParam : CommandParam<NPCWeightPair>
    {
        public NPCWeightPairParam(string name, string description, bool optional = false, NPCWeightPair defaultValue = null) :
    base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var splited = str.Split(':');
            var npc = TShock.Utils.GetNPCByIdOrName(splited[0]);
            if (npc == null || npc.Count == 0)
            {
                args.Player.SendErrorMessage("Failed found npc {0}!".SFormat(splited[0]));
                return false;
            }
            if (npc.Count > 1)
            {
                args.Player.SendInfoMessage("Found more than one npc!".SFormat(splited[0]));
                args.Player.SendInfoMessage(string.Join(", ", npc.Select(n => "({0}) {1}".SFormat(n.type.ToString(), n.FullName))));
                return false;
            }
            var weight = 1f;
            if(splited.Length > 1)
            {
                if (!float.TryParse(splited[1], out weight))
                {
                    args?.Player.SendErrorMessage("Invalid number '{0}' for '{1}'!".SFormat(str, Name));
                    return false;
                }
            }
            _value = new NPCWeightPair(npc[0], weight);
            return true;
        }
    }
}
