using System;
using System.Data;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension
{
    public class RegionExtManager
    {
        private IDbConnection database;

        public RegionExtManager (IDbConnection db)
        {
            database = db;
        }

        public bool Rename(string regionName, string newRegionName)
        {
            try
            {
                database.Query("UPDATE Regions SET RegionName=@0 WHERE RegionName=@1 AND WorldID=@2",
                    newRegionName, regionName, Main.worldID.ToString());
                var region = TShock.Regions.GetRegionByName(regionName);
                if (region != null)
                    region.Name = newRegionName;
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.ToString());
                return false;
            }
        }

        public bool ClearAllowUsers(string regionName)
        {
            Region r = TShock.Regions.GetRegionByName(regionName);
            if (r != null)
            {
                r.AllowedIDs.Clear();
                string ids = string.Join(",", r.AllowedIDs);
                return database.Query("UPDATE Regions SET UserIds=@0 WHERE RegionName=@1 AND WorldID=@2", ids,
                                       regionName, Main.worldID.ToString()) > 0;
            }
            return false;
        }
    }
}
