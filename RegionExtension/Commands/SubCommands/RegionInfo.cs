﻿using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    public class RegionInfo : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new string[] { "info", "i" };

        public override string Description => "Displays several information about the given region.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region info. Default: region in your location", true),
                new IntParam("page", "page of the list. Default: 1", true, 1)
            };
        }

        public RegionInfo(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)Params[0].Value;
            var page = ((IntParam)Params[1]).TValue;

            if (_checkRegionOwn && !CheckRegionOwn(args, region))
            {
                args.Player.SendErrorMessage("You cannot manage '{0}' region!".SFormat(region.Name));
                return;
            }
            SendRegionInfo(args, page, region);
        }

        private void SendRegionInfo(CommandArgsExtension args, int page, Region region)
        {
            var lines = Plugin.RegionExtensionManager.GetRegionInfo(region);
            var usedName = args.Message.Split(' ')[0];
            PaginationTools.SendPage(
                args.Player, page, lines, new PaginationTools.Settings
                {
                    HeaderFormat = string.Format("Information About Region \"{0}\" ({{0}}/{{1}}):", region.Name),
                    FooterFormat = string.Format("Type {0}{1} info {2} {{0}} for more information.", TShockAPI.Commands.Specifier, usedName, region.Name)
                }
            );
            var req = Plugin.RegionExtensionManager.RegionRequestManager.Requests.FirstOrDefault(r => r.Region.ID == region.ID);
            if (req != null && !Plugin.Config.AutoApproveRequest)
            {
                args.Player.SendInfoMessage("This region requires request confirmation!");
                args.Player.SendInfoMessage("Region will be deleted in '{0}'!".SFormat(req.DateCreation + StringTime.FromString(Plugin.Config.RequestTime)));
            }
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}
