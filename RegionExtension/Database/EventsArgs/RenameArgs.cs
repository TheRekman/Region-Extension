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
    public class RenameArgs : BaseRegionArgs
    {
        public RenameArgs(TSPlayer executor, Region region, string newName) :
            base(executor, region)
        {
            NewName = newName;
        }

        public string NewName { get; private set; }
    }
}
