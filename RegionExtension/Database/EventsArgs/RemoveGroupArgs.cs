using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Database.EventsArgs
{
    public class RemoveGroupArgs : BaseRegionArgs
    {
        public RemoveGroupArgs(TSPlayer userExecutor, Region region, Group group) :
            base(userExecutor, region)
        {
            Group = group;
        }

        public Group Group { get; private set; }
    }
}
