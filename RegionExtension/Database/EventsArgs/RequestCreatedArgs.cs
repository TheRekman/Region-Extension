using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Database.EventsArgs
{
    public class RequestCreatedArgs
    {
        public RequestCreatedArgs(Request req)
        {
            Request = req;
        }

        public Request Request { get; set; }
    }
}