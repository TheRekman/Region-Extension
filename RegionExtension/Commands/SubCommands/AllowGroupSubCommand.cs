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
    internal class AllowGroupSubCommand : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new[] { "allowg", "ag" };
        public override string Description => "Allow group to the region";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new GroupParam("group", "which group will be allowed."),
                new RegionParam("region", "in which region allowed. default: region in your location", true),
            };
        }


        public AllowGroupSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var userAccount = (Group)Params[0].Value;
            var region = (Region)Params[1].Value;
            if (_checkRegionOwn && !CheckRegionOwn(args, region))
            {
                args.Player.SendErrorMessage("You cannot manage '{0}' region!".SFormat(region.Name));
                return;
            }
            AllowGroup(args, userAccount, region);
        }

        private void AllowGroup(CommandArgsExtension args, Group group, Region region)
        {
            if (args.Plugin.RegionExtensionManager.AllowGroup(args, region, group))
                args.Player.SendSuccessMessage("Group '{0}' allowed to the region '{1}'".SFormat(group.Name, region.Name));
            else
                args.Player.SendErrorMessage("Failed allow group '{0}' to the region '{1}'!".SFormat(group.Name, region.Name));
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}
