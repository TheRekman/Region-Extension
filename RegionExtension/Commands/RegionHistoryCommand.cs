﻿using RegionExtension.Commands.SubCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Commands
{
    public class RegionHistoryCommand : CommandExtension
    {
        public override string[] Names => new[] { "regionhistory", "rh" };
        public override string[] Permissions => new[] { RegionExtension.Permissions.RegionHistoryCmd };
        public override string HelpText => "Manages region history.";

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new UndoSubCommand(),
            new RedoSubCommand(),
            new RestoreSubCommand(),
            new RestoreRegionUserSubCommand(),
            new GetHistorySubCommand(),
            new DeletedRegionsList()
        }.Concat(base.SubCommands).ToArray();
    }
}
