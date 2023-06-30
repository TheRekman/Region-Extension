using Microsoft.Xna.Framework;
using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    public class RestoreSubCommand : SubCommand
    {
        public override string[] Names => new[] { "restore", "res" };
        public override string Description => "restore region from deleted regions.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new StringParam("regionname", "name of region. Name must be exact same"),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (string)Params[0].Value;
            RestoreRegion(args, region);
        }

        private void RestoreRegion(CommandArgsExtension args, string region)
        {
            var reg = Plugin.RegionExtensionManager.DeletedRegions.GetRegionByName(region);
            if(reg == null)
            {
                args.Player.SendErrorMessage("Failed found region '{0}'!".SFormat(region));
                return;
            }
            Plugin.RegionExtensionManager.DeletedRegions.RemoveRegionFromDeleted(reg.Region.ID);
            string newName;
            if(!TryAutoComplete(region, reg.Region.Area, out newName))
            {
                args.Player.SendErrorMessage("Region '{0}' already exist!".SFormat(region));
                return;
            }
            reg.Region.Name = newName;
            if (Plugin.RegionExtensionManager.DefineRegion(args, reg.Region))
                args.Player.SendSuccessMessage("Region restored '{0}'!".SFormat(region));
            else
                args.Player.SendErrorMessage("Failed restore region!");
        }

        public bool TryAutoComplete(string str, Rectangle regionArea, out string result)
        {
            int num = 0;
            var reg = TShock.Regions.Regions.FirstOrDefault(r => r.Name.ToLower().Equals(str.ToLower()));
            var res = str;
            while (reg != null)
            {
                if (reg.Area.Equals(regionArea))
                {
                    result = null;
                    return false;
                }
                res = Plugin.Config.AutoCompleteSameNameFormat.SFormat(res, num);
                reg = TShock.Regions.Regions.FirstOrDefault(r => r.Name.ToLower().Equals(res.ToLower()));
                num++;
            }
            result = res;
            return true;
        }
    }
}
