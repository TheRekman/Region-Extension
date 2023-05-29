using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Database.EventsArgs
{
    public class RemoveArgs : BaseRegionArgs
    {
        public RemoveArgs(TSPlayer executor, Region region, UserAccount userName) :
            base (executor, region)
        {
            User = userName;
        }

        public UserAccount User { get; private set; }
    }
}
