using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Database.EventsArgs
{
    public class SetZArgs : BaseRegionArgs
    {
        public SetZArgs(TSPlayer executor, Region region, int amount) :
            base(executor, region)
        {
            Amount = amount;
        }

        public int Amount { get; private set; }
    }
}
