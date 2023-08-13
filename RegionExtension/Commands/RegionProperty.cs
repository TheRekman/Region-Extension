using RegionExtension.Commands.SubCommands;
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
        public override string[] Permissions => new[] { RegionExtension.Permissions.RegionPropertyCmd };
        public override string HelpText => "Manages region property.";

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new AddProperty(),
            new RemoveProperty(),
            new PropertyList(),
            new PropertyInfo(),
            new AddPropertyCondition(),
            new RemovePropertyCondition(),
            new ClearProperties()
        }.Concat(base.SubCommands).ToArray();
    }
}
