﻿using Microsoft.Xna.Framework;
using Terraria;
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
    internal class DefineRequestSubCommand : SubCommand
    {
        public override string[] Names => new[] { "define", "d" };
        public override string Description => "DefineRequestDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new StringParam("name", "name of new region."),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var regionname = (string)Params[0].Value;
            if (args.Player.TempPoints.Any(p => p == Point.Zero))
            {
                args.Player.SendErrorMessage("Set points before define region!");
                return;
            }
            if (!Utils.TryAutoComplete((string)Params[0].Value, out regionname))
            {
                args.Player.SendErrorMessage("Region '{0}' already exist!".SFormat(regionname));
                return;
            }
            DefineRegion(args, regionname);
        }

        private void DefineRegion(CommandArgsExtension args, string regionName)
        {
            var region = new Region()
            {
                Name = regionName,
                Area = new Rectangle(
                    Math.Min(args.Player.TempPoints[0].X, args.Player.TempPoints[1].X),
                    Math.Min(args.Player.TempPoints[0].Y, args.Player.TempPoints[1].Y),
                    Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X),
                    Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y)
                ),
                Owner = args.Player.Account.Name,
                WorldID = Main.worldID.ToString()
            };
            var checkResult = Utils.CheckConfigConditions(args.Player, region);
            if (!checkResult.res)
            {
                args.Player.SendErrorMessage(checkResult.msg);
                return;
            }
            if (Plugin.RegionExtensionManager.CreateRequest(region, args.Player))
            {
                args.Player.SendSuccessMessage("Region '{0}' defined!".SFormat(region.Name));
                args.Player.SendSuccessMessage("Request created!".SFormat(region.Name));
            }
            else
                args.Player.SendErrorMessage("Failed define region '{0}'!".SFormat(region.Name));
        }
    }
}
