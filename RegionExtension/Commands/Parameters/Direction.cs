namespace RegionExtension.Commands.Parameters
{
    public class Direction
    {
        private DirectionType _type;

        public DirectionType Type { get => _type; }

        public int TshockDirection {
            get 
            {
                if ((int)_type % 2 == 0)
                    return ((int)_type + 2) % 4;
                return (int)_type;
            }
        }

        public static DirectionType GetFromTshockDirection(int direction)
        {
            if (direction % 2 == 0)
                return (DirectionType)((direction + 2) % 4);
            return (DirectionType)direction;
        }

        public Direction(DirectionType type)
        {
            _type = type;
        }

        public (int x, int y) GetNewPosition(int x, int y, int amount)
        {
            
            int type = (int)_type;
            int sign = type >= 2 ? -1 : 1;
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
