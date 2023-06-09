using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension
{
    public class RequestSettings
    {
        public string GroupName = "default";
        public int MaxRequestCount = 3;
        public string RequestTime = "3d";
        public bool AutoApproveRequest = false;
        public int MaxRequestArea = 10000;
        public int MaxRequestHeight = 100;
        public int MaxRequestWidth = 100;
        public bool ProtectRequestedRegion = true;
        public int DefaultRequestZ = 0;
    }
}
