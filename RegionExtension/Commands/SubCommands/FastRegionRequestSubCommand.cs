using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    public class FastRegionRequestSubCommand : SubCommand
    {
        public override string[] Names => new[] { "fastregion", "fr" };
        public override string Description => "Create new region with two given point and params. Also send request.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new StringParam("region", "name of new region.")
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var regionname = (string)Params[0].Value;
            if (!TryAutoComplete((string)Params[0].Value, args, out regionname))
            {
                args.Player.SendErrorMessage("Region '{0}' already exist!".SFormat(regionname));
                return;
            }
            var username = (UserAccount)Params[1].Value;
            var z = ((IntParam)Params[2]).TValue;
            var protect = ((BoolParam)Params[3]).TValue;
            CreateFastRegionRequest(args, regionname, username, z, protect);
        }

        private void CreateFastRegionRequest(CommandArgsExtension args, string regionName, UserAccount username, int z, bool protect)
        {
            args.Plugin.FastRegions.Add(new FastRegion(args.Player, regionName, username.Name, z, protect, true));
        }

        public bool TryAutoComplete(string str, CommandArgsExtension args, out string result)
        {
            if (!Plugin.Config.AutoCompleteSameName)
            {
                result = str;
                return TShock.Regions.GetRegionByName(str) != null;
            }
            int num = 0;
            result = str;
            while (TShock.Regions.GetRegionByName(result) != null)
            {
                result = Plugin.Config.AutoCompleteSameNameFormat.SFormat(str, num);
                num++;
            }
            return true;
        }
    }
}
