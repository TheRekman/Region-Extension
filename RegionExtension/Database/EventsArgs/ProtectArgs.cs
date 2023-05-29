using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Database.EventsArgs
{
    public class ProtectArgs : BaseRegionArgs
    {
        public ProtectArgs(TSPlayer executor, Region region, bool protect) :
            base(executor, region)
        {
            Protect = protect;
        }

        public bool Protect { get; }
    }
}
