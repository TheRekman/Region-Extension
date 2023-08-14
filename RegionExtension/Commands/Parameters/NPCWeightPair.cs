using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace RegionExtension.Commands.Parameters
{
    public class NPCWeightPair
    {
        public NPCWeightPair(NPC npc, float weight)
        {
            NPCType = npc.type;
            Weight = weight;
        }

        public NPCWeightPair(string str)
        {
            var split = str.Split(':');
            NPCType = int.Parse(split[0]);
            Weight = float.Parse(split[1]);
        }

        public int NPCType { get; set; }
        public float Weight { get; set; }

        public override string ToString()
        {
            return string.Join(':', NPCType.ToString(), Weight.ToString());
        }
    }
}
