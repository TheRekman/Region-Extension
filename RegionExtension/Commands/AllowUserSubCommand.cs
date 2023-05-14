using RegionExtension.Commands.Parameters;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Commands
{
    public class AllowUserSubCommand : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new[] { "allow", "a" };
        public override string Description => "Allow user to the region";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {

                new UserAccountParam("useraccount", "which player will be allowed."),
                new RegionParam("region", "in which region allowed. default: region in your location", true),
            };
        }


        public AllowUserSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var userAccount = (UserAccount)Params[0].Value;
            var region = (Region)Params[1].Value;
            if (_checkRegionOwn && !CheckRegionOwn(args, region))
                return;
            AllowUser(args, userAccount, region);
        }

        private void AllowUser(CommandArgsExtension args, UserAccount userAccount, Region region)
        {
            if (TShock.Regions.AddNewUser(region.Name, userAccount.Name))
                args.Player.SendSuccessMessage("User '{0}' allowed to the region '{1}'".SFormat(userAccount.Name, region.Name));
            else
                args.Player.SendErrorMessage("Failed allow user '{0}' to the region '{1}'".SFormat(userAccount.Name, region.Name));
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}