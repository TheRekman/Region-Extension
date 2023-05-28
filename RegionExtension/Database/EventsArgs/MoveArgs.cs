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
    public class MoveArgs : BaseRegionArgs
    {
        public MoveArgs(TSPlayer executor, Region region, int amount, Direction direction) :
            base(executor, region)
        {
            Amount = amount;
            Direction = direction;
        }

        public int Amount { get; private set; }
        public Direction Direction { get; private set; }
    }
}
