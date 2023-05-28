using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Database.EventsArgs
{
    public class ChangeOwnerArgs : BaseRegionArgs
    {
        public ChangeOwnerArgs(TSPlayer userExecutor, Region region, UserAccount user) :
            base(userExecutor, region)
        {
            User = user;
        }

        public UserAccount User { get; private set; }
    }
}
