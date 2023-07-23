using IL.Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers
{
    public class Condition
    {
        public Condition(string name, Func<TSPlayer, Region, bool> predicate)
        {
            Name = name;
            Predicate = predicate;
        }

        public string Name { get; private set; }
        public Func<TSPlayer, Region, bool> Predicate { get; private set; }
    }
}
