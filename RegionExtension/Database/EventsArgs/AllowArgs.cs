using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Database.EventsArgs
{
    public class AllowArgs : BaseRegionArgs
    {
        public AllowArgs(TSPlayer userExecutor, Region region, UserAccount user) :
            base(userExecutor, region)
        {
            User = user;
        }
        public UserAccount User { get; private set; }
    }
}
