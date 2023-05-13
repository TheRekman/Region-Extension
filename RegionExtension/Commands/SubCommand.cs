using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands
{
    public abstract class SubCommand : ISubCommand
    {
        public virtual string[] Names => new[] { "name" };
        public virtual string Description => "descrition";
        public virtual ICommandParam[] Params => new ICommandParam[0];

        private IEnumerable<string> GetParamsStrings() =>
            Params.Select(p => "{0} - {1}".SFormat(p.Name, p.Description));

        private bool TryImplementParams(IEnumerable<string> parameters, CommandArgsExtension args)
        {
            var enumerator = parameters.GetEnumerator();
            var usedCommandName = args.Message.Split(' ')[0].Remove(0, 1);
            var usedSubCommandName = args.Parameters[0];
            for (int i = 0; i < Params.Length; i++)
                if (enumerator.MoveNext())
                {
                    if (!Params[i].TrySetValue(enumerator.Current, args))
                        return false;
                }
                else if (!Params[i].Optional)
                {
                    args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}{1} {2} {3}"
                                                 .SFormat(TShockAPI.Commands.Specifier, usedCommandName, usedSubCommandName,
                                                          string.Join(' ', Params.Select(p => p.GetBracketName()))));
                    return false;
                }
            return true;
        }

        public void InitializeCommand(CommandArgsExtension args)
        {
            if (TryImplementParams(args.Parameters.Skip(1), args))
                Execute(args);
        }

        public virtual void Execute(CommandArgsExtension args) =>
            args.Player.SendInfoMessage("Command not implemented");
    }
}
