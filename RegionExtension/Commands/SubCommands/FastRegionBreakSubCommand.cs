using RegionExtension.Commands.Parameters;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    internal class FastRegionBreakSubCommand : SubCommand
    {
        public override string[] Names => new[] { "fastregionbreak", "frb" };
        public override string Description => "breaks fast region request.";

        public override void Execute(CommandArgsExtension args)
        {
            var id = args.Plugin.FindFastRegionByUser(args.Player.Account);
            if (id == -1)
                args.Player.SendInfoMessage("You dont have fast region request.");
            else
            {
                args.Plugin.FastRegions.RemoveAt(id);
                args.Player.SendInfoMessage("Fast region request removed.");
            }
        }
    }
}