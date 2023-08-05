using RegionExtension.Commands;
using RegionExtension.Commands.Parameters;
using System;

namespace RegionExtension.RegionTriggers.Actions
{
    public class ActionFormer
    {
        public string Name { get; }
        public string Description { get; }
        public ICommandParam[] Params { get; }
        public Func<ICommandParam[], CommandArgsExtension, ITriggerAction> Action { get; }
        public Func<string, ITriggerAction> FromString { get; }

        public ActionFormer(string name, string description, ICommandParam[] @params, Func<ICommandParam[], CommandArgsExtension, ITriggerAction> action, Func<string, ITriggerAction> fromString)
        {
            Name = name;
            Description = description;
            Params = @params;
            Action = action;
            FromString = fromString;
        }

        public string Permission { get; set; }
    }
}