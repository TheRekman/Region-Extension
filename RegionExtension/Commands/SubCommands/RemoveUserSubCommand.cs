using RegionExtension.Commands.Parameters;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    public class RemoveUserSubCommand : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new[] { "remove", "r" };
        public override string Description => "Remove user from the region";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {

                new UserAccountParam("useraccount", "which player will be remove."),
                new RegionParam("region", "in which region remove. default: region in your location", true),
            };
        }


        public RemoveUserSubCommand(bool checkRegionOwn = false)
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
            RemoveUser(args, userAccount, region);
        }

        private void RemoveUser(CommandArgsExtension args, UserAccount userAccount, Region region)
        {
            if (args.Plugin.RegionExtensionManager.ChangeOwner(args, region, userAccount))
                args.Player.SendSuccessMessage("User '{0}' removed from the region '{1}'".SFormat(userAccount.Name, region.Name));
            else
                args.Player.SendErrorMessage("Failed remove user '{0}' from the region '{1}'".SFormat(userAccount.Name, region.Name));
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region) =>
            region.Owner == args.Player.Account.Name;
    }
}