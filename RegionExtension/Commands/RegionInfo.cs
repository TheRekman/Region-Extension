using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands
{
    public class RegionInfo : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new string[] { "info", "i" };

        public override string Description => "returns info about region.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("regionname", "which region info. Default: region in your location", true),
                new IntParam("page", "page of the list. Default: 1", true, 1)
            };
        }

        public RegionInfo(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var page = ((IntParam)Params[0]).TValue;
            var region = (Region)Params[1].Value;

            if (_checkRegionOwn && !CheckRegionOwn(args, region))
                return;
            SendRegionInfo(args, page, region);
        }

        private void SendRegionInfo(CommandArgsExtension args, int page, Region region)
        {
            var lines = new List<string>
                        {
                            string.Format("X: {0}; Y: {1}; W: {2}; H: {3}, Z: {4}", region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height, region.Z),
                            string.Concat("Owner: ", region.Owner),
                            string.Concat("Protected: ", region.DisableBuild.ToString()),
                        };

            if (region.AllowedIDs.Count > 0)
            {
                IEnumerable<string> sharedUsersSelector = region.AllowedIDs.Select(userId =>
                {
                    UserAccount user = TShock.UserAccounts.GetUserAccountByID(userId);
                    if (user != null)
                        return user.Name;

                    return string.Concat("{ID: ", userId, "}");
                });
                List<string> extraLines = PaginationTools.BuildLinesFromTerms(sharedUsersSelector.Distinct());
                extraLines[0] = "Shared with: " + extraLines[0];
                lines.AddRange(extraLines);
            }
            else
            {
                lines.Add("Region is not shared with any users.");
            }

            if (region.AllowedGroups.Count > 0)
            {
                List<string> extraLines = PaginationTools.BuildLinesFromTerms(region.AllowedGroups.Distinct());
                extraLines[0] = "Shared with groups: " + extraLines[0];
                lines.AddRange(extraLines);
            }
            else
            {
                lines.Add("Region is not shared with any groups.");
            }
            var usedName = args.Message.Split(' ')[0];
            PaginationTools.SendPage(
                args.Player, page, lines, new PaginationTools.Settings
                {
                    HeaderFormat = string.Format("Information About Region \"{0}\" ({{0}}/{{1}}):", region.Name),
                    FooterFormat = string.Format("Type {0}/ro info {1} {{0}} for more information.", TShockAPI.Commands.Specifier, region.Name)
                }
            );
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}
