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
        public override string Description => "Create new region with two given point and params.";

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
            if (!TryAutoComplete((string)Params[0].Value, args, out regionname))
            {
                args.Player.SendErrorMessage("Region '{0}' alreadt exist!".SFormat(regionname));
                return;
            }
            var username = (UserAccount)Params[1].Value;
            var z = ((IntParam)Params[2]).TValue;
            var protect = ((BoolParam)Params[3]).TValue;
            CreateFastRegionRequest(args, regionname, username, z, protect);
        }

        private void CreateFastRegionRequest(CommandArgsExtension args, string regionName, UserAccount username, int z, bool protect)
        {
            args.Plugin.FastRegions.Add(new FastRegion(args.Plugin, args.Player, regionName, username.Name, z, protect));
        }

        public bool TryAutoComplete(string str, CommandArgsExtension args, out string result)
        {
            if (!args.Plugin.Config.AutoCompleteSameName)
            {
                result = str;
                return TShock.Regions.GetRegionByName(str) != null;
            }
            int num = 0;
            result = str;
            while (TShock.Regions.GetRegionByName(result) != null)
            {
                result = args.Plugin.Config.AutoCompleteSameNameFormat.SFormat(str, num);
                num++;
            }
            return true;
        }
    }
}