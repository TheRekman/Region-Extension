using System;
using TShockAPI;
using TShockAPI.DB;
using Microsoft.Xna.Framework;

namespace RegionExtension
{
    public class FastRegion
    {
        
        private int X1, Y1, X2, Y2;
        private int point;
        private readonly TSPlayer _player;
        private readonly string _regionName;
        private readonly string _ownerName;
        private readonly int _z;
        private readonly bool _protect;

        public UserAccount User { get { return _player.Account; } }

        public FastRegion(TSPlayer player, string regionName, string ownerName, int z = 0, bool protect = true)
        {
            _player = player;
            _regionName = regionName;
            _ownerName = ownerName;
            _z = z;
            _protect = protect;
            _player.SendInfoMessage("Hit tile to Set Points, or use The Grand Design.");
        }

        public bool SetPoint(int x, int y)
        {
            if(point == 0)
            {
                X1 = x;
                Y1 = y;
                _player.SendInfoMessage("Set point 1.");
                point = 1;
                return false;
            }
            else
            {
                X2 = x;
                Y2 = y;
                _player.SendInfoMessage("Set point 2.");
                CreateRegion();
                return true;
            }
        }

        public bool SetPoints(int x1, int y1, int x2, int y2)
        {
            if ((x1 == x2 && y1 == y2) || point == 1) return SetPoint(x1, y1);
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            _player.SendInfoMessage("Set point 1 & 2.");
            CreateRegion();
            return true;
        }

        private void CreateRegion()
        {
            var x = Math.Min(X1, X2);
            var y = Math.Min(Y1, Y2);
            var width = Math.Abs(X1 - X2);
            var height = Math.Abs(Y1 - Y2);
            var region = new Region()
            {
                Area = new Rectangle(x, y, width, height),
                Name = _regionName,
                Owner = _ownerName,
                WorldID = Terraria.Main.worldID.ToString(),
                Z = _z,
                DisableBuild = _protect
            };

            if(Plugin.RegionExtensionManager.DefineRegion(_player, region))
            {
                _player.SendSuccessMessage("Set region " + _regionName);
            }
            
        }
    }
}
