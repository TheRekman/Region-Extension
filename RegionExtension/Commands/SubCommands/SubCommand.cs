using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    public abstract class SubCommand : ISubCommand
    {
        protected ICommandParam[] _params;

        public virtual string[] Names => new[] { "NOT_DEFINED" };
        public virtual string Description => "NOT_DEFINED";
        public ICommandParam[] Params
        {
            get
            {
                if (_params == null)
                    InitializeParams();
                return _params;
            }
        }

        public virtual void InitializeParams()
        {
            _params = new ICommandParam[0];
        }

        private bool TryImplementParams(IEnumerable<string> parameters, CommandArgsExtension args)
        {
            var enumerator = parameters.GetEnumerator();
            var usedCommandName = args.Message.Split(' ')[0];
            var usedSubCommandName = args.Parameters.Count > 0 ? args.Parameters[0] : Names[0];
            InitializeParams();
            var paramsCopy = new List<ICommandParam>(_params);
            var dynamicParams = new List<ICommandParam>();
            for (int i = 0; i < paramsCopy.Count; i++)
            {
                if (enumerator.MoveNext())
                {
                    if (!paramsCopy[i].TrySetValue(enumerator.Current, args))
                        return false;
                    if (paramsCopy[i].Dynamic)
                    {
                        if (paramsCopy[i].Parametrical)
                        {
                            if (i != paramsCopy.Count - 1)
                                throw new ArgumentException("Parametrical param must be last!");
                            while (enumerator.MoveNext())
                                if (!paramsCopy[i].TrySetValue(enumerator.Current, args))
                                    return false;
                            break;
                        }
                        dynamicParams.Add(paramsCopy[i]);
                        var temp = paramsCopy.Take(i + 1).ToList();
                        temp.AddRange(paramsCopy[i].GetAdditionalParams());
                        temp.AddRange(paramsCopy.Skip(i + 1));
                        paramsCopy = temp;
                    }
                }
                else if (!paramsCopy[i].TrySetDefaultValue(args))
                {
                    args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}{1} {2} {3}"
                                                    .SFormat(TShockAPI.Commands.Specifier, usedCommandName, usedSubCommandName,
                                                            string.Join(' ', paramsCopy.Where(p => !string.IsNullOrEmpty(p.Name))
                                                                                       .Select(p => p.GetBracketName()))));
                    return false;
                }
            }
            foreach (var param in dynamicParams)
                if (!param.TrySetDynamicValue(args))
                    return false;
            foreach (var param in paramsCopy)
                param.Clear();
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
