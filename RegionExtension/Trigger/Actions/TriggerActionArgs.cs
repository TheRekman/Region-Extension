using IL.Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Trigger.Actions
{
    public class TriggerActionArgs
    {
        public TriggerActionArgs(TSPlayer player, Region region)
        {
            Player = player;
            Region = region;
        }

        public TSPlayer Player { get; }
        public Region Region { get; }
        public bool Handled { get; set; }
    }
}
