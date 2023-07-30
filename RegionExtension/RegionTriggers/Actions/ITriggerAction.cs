using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.RegionTriggers.Actions
{
    public interface ITriggerAction
    {
        public string Name { get; }
        public string Description { get; }
        public static ActionFormer Former { get; }
        public void Execute(TriggerActionArgs args);
        public string GetArgsString();
    }
}
