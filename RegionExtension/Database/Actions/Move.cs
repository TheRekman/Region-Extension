using RegionExtension.Commands.Parameters;
using RegionExtension.Database.EventsArgs;
using System;
using System.Collections.Generic;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class Move : IAction
    {
        private string _regionName;
        private int _amount;
        private DirectionType _direction;

        public string Name => ActionFactory.MoveName;

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

        public Move(string regionName, int amount, DirectionType direction)
        {
            _regionName = regionName;
            _amount = amount;
            _direction = direction;
        }

        public Move(MoveArgs args)
        {
            _regionName = args.Region.Name;
            _amount = args.Amount;
            _direction = args.Direction.Type;
        }

        public Move(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _amount = int.Parse(args[1]);
            _direction = Enum.Parse<DirectionType>(args[2]);
        }

        public void Do()
        {
            Direction direction = new Direction(_direction);
            var region = TShock.Regions.GetRegionByName(_regionName);
            var newPos = direction.GetNewPosition(region.Area.X, region.Area.Y, _amount);
            TShock.Regions.PositionRegion(_regionName, newPos.x, newPos.y, region.Area.Width, region.Area.Height);
        }

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new Move(undoString);

        public string GetUndoArgsString()
        {
            int amount = _amount;
            DirectionType direction = (DirectionType)((int)(_direction + 2) % 4);
            return string.Join(' ', _regionName, amount, direction);
        }

        public IEnumerable<string> GetInfoString() =>
            new string[]
            {
                Name + ": ",
                _amount.ToString(),
                _direction.ToString()
            };
    }
}
