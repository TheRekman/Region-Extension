using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    internal class SetZ : IAction
    {
        private string _regionName;
        private int _z;

        public string Name => "SetZ";

        public object[] Params
        {
            get
            {
                return new object[]
                {
                    _regionName, _z
                };
            }
        }

        public SetZ(string regionName, int z)
        {
            _regionName = regionName;
            _z = z;
        }

        public SetZ(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _z = int.Parse(args[1]);
        }

        public void Do() =>
            TShock.Regions.SetZ(_regionName, _z);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new SetZ(undoString);

        public string GetUndoArgsString()
        {
            int z = TShock.Regions.GetRegionByName(_regionName).Z;
            return string.Join(' ', _regionName, z);
        }
    }
}
