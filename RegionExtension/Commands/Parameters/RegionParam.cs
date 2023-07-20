using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.Parameters
{
    public class RegionParam : CommandParam<Region>
    {
        public RegionParam(string name, string description, bool optional = false, Region defaultValue = null) :
            base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var region = TShock.Regions.GetRegionByName(str);
            if (region == null)
            {
                args?.Player.SendInfoMessage("Failed found region '{0}'! Trying found with this start.".SFormat(str));
                var foundedRegions = TShock.Regions.Regions.Where(r => r.Name.ToLower().StartsWith(str.ToLower()));
                if(foundedRegions.Count() > 1)
                {
                    args.Player.SendInfoMessage("Founded regions: ");
                    args.Player.SendInfoMessage(string.Join(", ", foundedRegions.Select(r => r.Name)));
                    return false;
                }
                region = foundedRegions.FirstOrDefault();
            }
            if (region == null)
            {
                args?.Player.SendErrorMessage("Invalid region '{0}'!".SFormat(str));
                return false;
            }
            _value = region;
            return true;
        }

        public override bool TrySetDefaultValue(CommandArgsExtension args = null)
        {
            if (args.Player.CurrentRegion == null)
                return false;
            _defaultValue = args.Player.CurrentRegion;
            return base.TrySetDefaultValue(args);
        }
    }
}
