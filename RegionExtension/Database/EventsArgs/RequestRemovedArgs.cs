using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Database.EventsArgs
{
    public class RequestRemovedArgs
    {
        public RequestRemovedArgs(TSPlayer user, Request req, bool approved)
        {
           Remover = user;
           Request = req;
           Approved = approved;
        }

        public TSPlayer User { get; set; }
        public Request Request { get; set; }
        public bool Approved { get; set; }
    }
}
