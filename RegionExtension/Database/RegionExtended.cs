using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace RegionExtension.Database
{
    public class RegionExtended
    {
        public Region Region { get; set; }
        public RegionExtensionInfo ExtensionInfo { get; set; }
    }
}
