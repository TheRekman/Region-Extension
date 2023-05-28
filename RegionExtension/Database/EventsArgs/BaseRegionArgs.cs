using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Database.EventsArgs
{
    public class BaseRegionArgs
    {
        public TSPlayer UserExecutor { get; private set; }
        public Region Region { get; private set; }


        public BaseRegionArgs(TSPlayer userExecutor, Region region)
        {
            UserExecutor = userExecutor;
            Region = region;
        }
    }
}
