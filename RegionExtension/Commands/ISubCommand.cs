using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegionExtension.Commands.Parameters;
using TShockAPI;

namespace RegionExtension.Commands
{
    public interface ISubCommand
    {
        string[] Names { get; }
        string Description { get; }
        ICommandParam[] Params { get; }
        void InitializeCommand(CommandArgsExtension args);
    }
}
