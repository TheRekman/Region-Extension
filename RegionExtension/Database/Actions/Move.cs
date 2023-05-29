using RegionExtension.Database.EventsArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    internal class Move : IAction
    {
        private string _regionName;
        private int _amount;
        private int _direction;

        public string Name => "Move";

        public object[] Params
        {
            get
            {
                return new object[]
                {
                    _regionName, _amount, _direction
                };
            }
        }

        public Move(string regionName, int amount, int direction)
        {
            _regionName = regionName;
            _amount = amount;
            _direction = direction;
        }

        public Move(MoveArgs args)
        {
            _regionName = args.Region.Name;
            _amount = args.Amount;
            _direction = args.Direction;
        }

        public Move(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _amount = int.Parse(args[1]);
            _direction = int.Parse(args[2]);
        }

        public void Do() =>
            TShock.Regions.ResizeRegion(_regionName, _amount, _direction);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new Resize(undoString);

        public string GetUndoArgsString()
        {
            int amount = _amount;
            int direction = _direction;
            return string.Join(' ', _regionName, amount, direction);
        }
    }
}
