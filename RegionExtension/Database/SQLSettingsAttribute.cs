using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Database
{
    public class SQLSettingAttribute : Attribute
    {
        public SQLSettingAttribute(params SqlColumnSettings[] settings)
        {
            Settings = settings;
        }

        public SqlColumnSettings[] Settings { get; private set; }
    }

    public enum SqlColumnSettings
    {
        Unique,
        Primary,
        AutoIncrement,
        NotNull,
        DefaultCurrentTimestamp
    }
}
