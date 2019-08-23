using System;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension
{
    class FastRegion
    {
        TSPlayer Player;
        public User User { get { return Player.User; } }
        int X1, Y1, X2, Y2;
        int point;
        string RegionName;
        string OwnerName;
        int Z;
        bool Protect;
        int Stage = 0;

        public FastRegion(TSPlayer player, string regionName, string ownerName, int z = 0, bool protect = true)
        {
            Player = player;
            RegionName = regionName;
            OwnerName = ownerName;
            Z = z;
            Protect = protect;
            Player.SendInfoMessage("Hit tile to Set Points, or use The Grand Design.");
        }

        public bool SetPoint(int x, int y)
        {
            if(point == 0)
            {
                X1 = x;
                Y1 = y;
                Player.SendInfoMessage("Set point 1.");
                point = 1;
                return false;
            }
            else
            {
                X2 = x;
                Y2 = y;
                Player.SendInfoMessage("Set point 2.");
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
            Player.SendInfoMessage("Set point 1 & 2.");
            CreateRegion();
            return true;
        }
        private void CreateRegion()
        {
            var x = Math.Min(X1, X2);
            var y = Math.Min(Y1, Y2);
            var width = Math.Abs(X1 - X2);
            var height = Math.Abs(Y1 - Y2);

            if(TShock.Regions.AddRegion(x, y, width, height, RegionName, OwnerName, Terraria.Main.worldID.ToString(), Z) &&
            TShock.Regions.SetRegionState(RegionName, Protect))
            {
                Player.SendSuccessMessage("Set region " + RegionName);
            }
            
        }
    }
}
