using RegionExtension.Commands.SubCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Commands
{
    public class RegionTriggerCommand : CommandExtension
    {
        public override string[] Names => new[] { "regiontrigger", "rt" };
        public override string[] Permissions => new[] { "regionext.trigger" };
        public override string HelpText => "Manages region trigger.";

        public override ISubCommand[] SubCommands =>
            new ISubCommand[]
        {
            new AddTrigger(),
            new DeleteTrigger(),
            new TriggerList(),
            new ConditionList(),
            new AddTriggerCondition(),
            new RemoveTriggerCondition()
        }.Concat(base.SubCommands).ToArray();
    }
}
