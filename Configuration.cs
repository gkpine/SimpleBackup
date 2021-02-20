using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace SimpleBackup
{
    public class Configuration
    {
        public bool MinimizeToTray { get; set; }
        public float BackupEveryMins { get; set; }
        public List<string> FilesToBackup { get; set; }
        public List<string> BackupLocations { get; set; }

        public Configuration()
        {
            this.MinimizeToTray = true;
            this.BackupEveryMins = 30;
            this.FilesToBackup = new List<string>();
            this.BackupLocations = new List<string>();
        }

        public Configuration(string jsonLoc)
        {
            this.LoadConfig(jsonLoc);
        }

        public bool SaveConfig(string saveLoc)
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(saveLoc, json);
                return true;
            } 
            catch (Exception ex)
            {
                return false;
            }
        }

        public void LoadConfig(string path)
        {
            string json = File.ReadAllText(path);
            Configuration config = JsonConvert.DeserializeObject<Configuration>(json);
            this.MinimizeToTray = config.MinimizeToTray;
            this.BackupEveryMins = config.BackupEveryMins;
            this.FilesToBackup = config.FilesToBackup;
            this.BackupLocations = config.BackupLocations;
        }

    }
}
