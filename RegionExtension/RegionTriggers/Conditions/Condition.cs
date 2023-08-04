using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.Conditions
{
    public class Condition
    {
        public Condition(string[] names, bool reversed, Func<TSPlayer, Region, bool> predicate)
        {
            Names = names;
            Predicate = predicate;
            IsReversed = reversed;
        }
        public string[] Names { get; }
        Func<TSPlayer, Region, bool> Predicate { get; }
        public bool IsReversed { get; } = false;
        public bool Check(TSPlayer player, Region region)
        {
            bool res = Predicate(player, region);
            if(IsReversed)
                res = !res;
            return res;
        }
    }
}
