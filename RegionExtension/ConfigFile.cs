using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TShockAPI;

namespace RegionExtension
{
    class ConfigFile
    {
        [JsonIgnore]
        private static readonly string Path = System.IO.Path.Combine(TShock.SavePath, "RegionExtention.json");

        public string ContextSpecifier = "$";
        public bool ContextAllow = true;
        public bool AutoCompleteSameName = true;
        public string AutoCompleteSameNameFormat = "{0}:{1}"; //{0} - region name, {1} - region number

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