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
            if (!Utils.TryAutoComplete((string)Params[0].Value, out regionname))
            {
                args.Player.SendErrorMessage("Region '{0}' already exist!".SFormat(regionname));
                return;
            }
            CreateFastRegionRequest(args, regionname, args.Player.Account);
        }

        private void CreateFastRegionRequest(CommandArgsExtension args, string regionName, UserAccount username)
        {
            args.Plugin.FastRegions.Add(new FastRegion(args.Player, regionName, username.Name, 0, true, true));
        }
    }
}
