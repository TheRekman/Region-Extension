using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.Parameters
{
    public class WarpParam : CommandParam<Warp>
    {
        public WarpParam(string name, string description, bool optional = false, Warp defaultValue = null) :
            base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var warp = TShock.Warps.Warps.Where(w => w.Name.ToLower().Equals(str.ToLower()));
            if (warp == null || warp.Count() < 1)
            {
                args?.Player.SendInfoMessage("Failed found warp '{0}'!".SFormat(str));
                return false;
            }
            if (warp.Count() > 1)
            {
                args.Player.SendInfoMessage("Founded warps: ");
                args.Player.SendInfoMessage(string.Join(", ", warp.Select(warp => warp.Name)));
                return false;
            }
            _value = warp.First();
            return true;
        }
    }
}
