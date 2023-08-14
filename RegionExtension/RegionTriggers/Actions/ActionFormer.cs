using RegionExtension.Commands;
using RegionExtension.Commands.Parameters;
using System;

namespace RegionExtension.RegionTriggers.Actions
{
    public class ActionFormer
    {
        public string[] Names { get; }
        public string Description { get; }
        public ICommandParam[] Params { get; }
        public Func<ICommandParam[], CommandArgsExtension, ITriggerAction> Action { get; }
        public Func<string, ITriggerAction> FromString { get; }

        public ActionFormer(string[] names, string description, ICommandParam[] @params, Func<ICommandParam[], CommandArgsExtension, ITriggerAction> action, Func<string, ITriggerAction> fromString)
        {
            Names = names;
            Description = description;
            Params = @params;
            Action = action;
            FromString = fromString;
        }

        public string Permission { get; set; }
    }
}