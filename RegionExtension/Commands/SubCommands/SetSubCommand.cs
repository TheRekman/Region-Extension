using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.Localization;

namespace RegionExtension.Commands.SubCommands
{
    public class SetSubCommand : SubCommand
    {
        public override string[] Names => new string[] { "set" };

        public override string Description => "Define point.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new IntParam("1/2", "point of set.")

            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var point = (int)Params[0].Value;
            if (point != 1 || point != 2)
            {
                args.Player.SendErrorMessage("Invalid point number! 1/2 only");
                return;
            }
            SetPoint(args, point);
        }

        private void SetPoint(CommandArgsExtension args, int point)
        {
            args.Player.SendInfoMessage("Hit a block to set point {0}.".SFormat(point));
            args.Player.AwaitingTempPoint = point;
        }
    }
}
