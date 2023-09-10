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
        public RequestRemovedArgs(UserAccount user, Request req, bool approved)
        {
           User = user;
           Request = req;
           Approved = approved;
        }

        public UserAccount User { get; set; }
        public Request Request { get; set; }
        public bool Approved { get; set; }
    }
}
