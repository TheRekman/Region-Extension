using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Database.EventsArgs
{
    public class AllowGroupArgs : BaseRegionArgs
    {
        public AllowGroupArgs(TSPlayer userExecutor, Region region, Group group) :
            base(userExecutor, region)
        {
            Group = group;
        }

        public Group Group { get; private set; }
    }
}
