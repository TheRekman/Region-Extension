using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace RegionExtension.Commands
{
    public class ClearMembersSubCommand : SubCommand
    {

        public bool _checkRegionOwn;

        public override string[] Names => new[] { "clearmembers", "cm" };
        public override string Description => "remove all members from region.";

        public override ICommandParam[] Params => new ICommandParam[]
        {
            new RegionParam("regionname", "region in which members will be cleared. default: your location region", true)
        };

        public ClearMembersSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)Params[0].Value;
            if (_checkRegionOwn && !CheckRegionOwn(args, region))
                return;
            ClearMembersInRegion(args, region);
            base.Execute(args);
        }

        private void ClearMembersInRegion(CommandArgsExtension args, Region region)
        {
            if(args.Plugin.ExtManager.ClearAllowUsers(region.Name))
                args.Player.SendSuccessMessage("All users cleared from region.");
            else
                args.Player.SendErrorMessage("Failed clear users.");
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;

    }
}
