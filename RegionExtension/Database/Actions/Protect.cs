using RegionExtension.Database.EventsArgs;
using System.Collections.Generic;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class Protect : IAction
    {
        private string _regionName;
        private bool _protect;

        public string Name => ActionFactory.ProtectName;

        public object[] Params
        {
            get
            {
                return new object[]
                {
                    _regionName, _protect
                };
            }
        }

        public Protect(string regionName, bool protect)
        {
            _regionName = regionName;
            _protect = protect;
        }

        public Protect(ProtectArgs args)
        {
            _regionName = args.Region.Name;
            _protect = args.Protect;
        }

        public Protect(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _protect = bool.Parse(args[1]);
        }

        public void Do() =>
            TShock.Regions.SetRegionState(_regionName, _protect);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new Protect(undoString);

        public string GetUndoArgsString()
        {
            bool protect = TShock.Regions.GetRegionByName(_regionName).DisableBuild;
            return string.Join(' ', _regionName, protect);
        }

        public IEnumerable<string> GetInfoString() =>
            new string[]
            {
                Name + ":",
                _regionName,
                _protect.ToString()
            };
    }
}
