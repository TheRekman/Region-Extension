using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Database
{
    public static class UtilsDB
    {
        public static bool UpdateQuery(IDbConnection db, string table, string collumn, string value, string targetCollumn, string targetId)
        {
            try
            {
                db.Query("UPDATE @0 SET @1=@2 WHERE @3=@4", table, collumn, value, targetCollumn, targetId);
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }
    }
}
