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
    public class SetOwnerSubCommand : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new[] { "setowner", "so" };
        public override string Description => "Set region owner";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new UserAccountParam("useraccount", "which player be defined as owner. default: your user account", true),
                new RegionParam("region", "which region owner be rewriten. default: region in your location", true)
            };
        }


        public SetOwnerSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var userAccount = (UserAccount)Params[0].Value;
            var region = (Region)Params[1].Value;
            if (_checkRegionOwn && !CheckRegionOwn(args, region))
            {
                args.Player.SendErrorMessage("You cannot manage '{0}' region!".SFormat(region.Name));
                return;
            }
            SetRegionOwn(args, userAccount, region);
        }

        private void SetRegionOwn(CommandArgsExtension args, UserAccount userAccount, Region region)
        {
            if (args.Plugin.ExtManager.ChangeOwner(args, region, userAccount))
                args.Player.SendSuccessMessage("Region changeowner success!");
            else
                args.Player.SendErrorMessage("Region changeowner failed!");
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}
