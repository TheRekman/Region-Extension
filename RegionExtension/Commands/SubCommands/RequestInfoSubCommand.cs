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
    internal class RequestInfoSubCommand : SubCommand
    {

        public override string[] Names => new string[] { "requestinfo", "ri" };

        public override string Description => "Displays several information about the given request.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region request info. Default: region in your location", true),
                new IntParam("page", "page of the list. Default: 1", true, 1)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)Params[0].Value;
            var page = ((IntParam)Params[1]).TValue;
            SendRequestInfo(args, page, region);
        }

        private void SendRequestInfo(CommandArgsExtension args, int page, Region region)
        {
            var request = Plugin.RegionExtensionManager.RegionRequestManager.Requests.FirstOrDefault(r => r.Region.ID == region.ID);
            if(request == null)
            {
                args.Player.SendInfoMessage($"Region {region.Name} dont have request.");
                return;
            }
            var usedName = args.Message.Split(' ')[0];
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(
                args.Player, page, request.GetInfoStrings().ToList(), new PaginationTools.Settings
                {
                    HeaderFormat = string.Format("Information About Request \"{0}\" ({{0}}/{{1}}):", region.Name),
                    FooterFormat = string.Format("Type {0}{1} {2} {3} {{0}} for more information.", TShockAPI.Commands.Specifier, usedName, usedSubCommandName, region.Name)
                }
            );
        }
    }
}
