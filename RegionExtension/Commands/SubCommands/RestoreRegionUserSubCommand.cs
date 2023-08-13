using Microsoft.Xna.Framework;
using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    internal class RestoreRegionUserSubCommand : SubCommand
    {
        public override string[] Names => new[] { "restoreuser", "resu" };
        public override string Description => "RegionRestoreByUserDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new UserAccountParam("user", "User deletion."),
                new IntParam("count", "count of regions. Name must be exact same. Default: 1", true, 1)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var user = (UserAccount)Params[0].Value;
            var count = (int)Params[1].Value;
            RestoreRegion(args, user, count);
        }

        private void RestoreRegion(CommandArgsExtension args, UserAccount target, int count)
        {
            var regions = Plugin.RegionExtensionManager.DeletedRegions.GetRegionsByUser(target);
            if (regions == null || regions.Count == 0)
            {
                args.Player.SendErrorMessage("Failed found region by user '{0}'!".SFormat(target.Name));
                return;
            }
            Task.Run(() =>
            {
                var restoredCount = 0;
                foreach (var reg in regions)
                {
                    if (count == 0)
                        break;
                    Plugin.RegionExtensionManager.DeletedRegions.RemoveRegionFromDeleted(reg.Region.ID);
                    string newName;
                    if (!TryAutoComplete(reg.Region.Name, reg.Region.Area, out newName))
                    {
                        args.Player.SendErrorMessage("Region '{0}' already exist!".SFormat(reg.Region.Name));
                        continue;
                    }
                    reg.Region.Name = newName;
                    if (!Plugin.RegionExtensionManager.DefineRegion(args, reg.Region))
                        args.Player.SendErrorMessage("Failed restore region '{0}'!".SFormat(reg.Region.Name));
                    restoredCount++;
                    count--;
                }
                args.Player.SendSuccessMessage("'{0}' regions restored!".SFormat(restoredCount));
            });
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
