using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class Protect : IAction
    {
        private string _regionName;
        private bool _protect;

        public string Name => "Protect";

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
    }
}
