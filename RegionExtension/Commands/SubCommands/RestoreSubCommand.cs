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
                new StringParam("regionname", "name of region. Name must be exact same", true),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (string)Params[0].Value;
            RestoreRegion(args, region);
        }

        private void RestoreRegion(CommandArgsExtension args, string region)
        {
            var reg = args.Plugin.RegionExtensionManager.DeletedRegions.GetRegionByName(region);
            if(reg == null)
            {
                args.Player.SendErrorMessage("Failed found region '{0}'!".SFormat(region));
                return;
            }
            args.Plugin.RegionExtensionManager.DeletedRegions.DeleteRegion(reg.Region.ID);
            string newName;
            TryAutoComplete(region, args, out newName);
            reg.Region.Name = newName;
            if (args.Plugin.RegionExtensionManager.DefineRegion(args, reg.Region))
                args.Player.SendSuccessMessage("Region restored '{0}'!".SFormat(region));
            else
                args.Player.SendErrorMessage("Failed restore region!");
        }

        public bool TryAutoComplete(string str, CommandArgsExtension args, out string result)
        {
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
