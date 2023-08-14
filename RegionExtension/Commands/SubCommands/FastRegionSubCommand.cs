using RegionExtension.Commands.Parameters;
using System;
using Terraria.GameContent.UI.States;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    public class FastRegionSubCommand : SubCommand
    {
        public override string[] Names => new[] { "fastregion", "fr" };
        public override string Description => "FastRegionDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new StringParam("region", "name of new region."),
                new UserAccountParam("username", "owner of new region. default: your user account.", true),
                new IntParam("z", "region priority. default: 0", true, 0),
                new BoolParam("protect", "region protect. default: true", true, true)
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
            var username = (UserAccount)Params[1].Value;
            var z = ((IntParam)Params[2]).TValue;
            var protect = ((BoolParam)Params[3]).TValue;
            CreateFastRegionRequest(args, regionname, username, z, protect);
        }

        private void CreateFastRegionRequest(CommandArgsExtension args, string regionName, UserAccount username, int z, bool protect)
        {
            args.Plugin.FastRegions.Add(new FastRegion(args.Player, regionName, username.Name, z, protect));
        }
    }
}