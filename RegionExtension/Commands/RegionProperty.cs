﻿using RegionExtension.Commands.SubCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Commands
{
    internal class RegionProperty : CommandExtension
    {
        public override string[] Names => new[] { "regionproperty", "rp" };
        public override string[] Permissions => new[] { "regionext.property" };
        public override string HelpText => "Manages region property.";

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new AddProperty(),
            new RemoveProperty(),
            new PropertyList(),
            new PropertyInfo()
        }.Concat(base.SubCommands).ToArray();
    }
}