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
        Region Region { get; set; }
        RegionExtensionInfo ExtensionInfo { get; set; }
    }
}
