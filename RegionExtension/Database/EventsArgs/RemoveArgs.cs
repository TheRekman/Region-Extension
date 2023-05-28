using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace RegionExtension.Database.EventsArgs
{
    public class RemoveArgs : BaseRegionArgs
    {
        public RemoveArgs(UserAccount executor, Region regionName, UserAccount userName) : base
        {
            RegionName = regionName;
            User = userName;
        }

        public Region RegionName { get; private set; }
        public UserAccount User { get; private set; }
    }
}
