using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Commands.Parameters
{
    public class Direction
    {
        private DirectionType _type;

        public Direction(DirectionType type)
        {
            _type = type;
        }

        public (int x, int y) GetNewPosition(int x, int y, int amount)
        {
            int type = (int)_type;
            int sign = type > 2 ? -1 : 1;
            x += type % 2 * amount * sign;
            y += (type + 1) % 2 * amount * sign;
            return new(x, y);
        }
    }

    public enum DirectionType
    {
        Down,
        Right,
        Up,
        Left
        
    }
}
