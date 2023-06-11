using System;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace RegionExtension
{
    public class ConfigFile
    {
        [JsonIgnore]
        private static readonly string Path = System.IO.Path.Combine(TShock.SavePath, "RegionExtension.json");

        public string ContextSpecifier = "$";
        public bool ContextAllow = true;
        public bool AutoCompleteSameName = true;
        public string AutoCompleteSameNameFormat = "{0}:{1}"; //{0} - region name, {1} - region number
        public string NotificationPeriod = "10m";
        public RequestSettings[] RequestSettings = new RequestSettings[]
            {
                new RequestSettings {},
                new RequestSettings
                {
                    GroupName = "superadmin",
                    MaxRequestCount = 0,
                    RequestTime = "0s",
                    AutoApproveRequest = true,
                    MaxRequestArea = 0,
                    MaxRequestHeight = 0,
                    MaxRequestWidth = 0,
                    ProtectRequestedRegion = true,
                    DefaultRequestZ = 0
                }
            };

        public static ConfigFile Read()
        {
            if (!File.Exists(Path))
                return Create();    
            try
            {
                string jsonString = File.ReadAllText(Path);
                var conf = JsonConvert.DeserializeObject<ConfigFile>(jsonString);
                return conf;
            }
            catch (Exception e)
            {
                TShock.Log.ConsoleError("[RegionExt] Failed read config file.");
                TShock.Log.ConsoleError(e.Message);
                return new ConfigFile();
            }
        }
        public static ConfigFile Create()
        {
            try
            {
                var conf = new ConfigFile();
                File.WriteAllText(Path, JsonConvert.SerializeObject(conf, Formatting.Indented));
                TShock.Log.Info("[RegionExt] New config file created.");
                return conf;
            }
            catch
            {
                TShock.Log.ConsoleError("[RegionExt] Failed create config file.");
                throw;
            }

        }
    }
}