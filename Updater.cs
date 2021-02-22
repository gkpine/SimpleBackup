using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SimpleBackup
{
    public class UpdateInfo
    {
        public float Version { get; set; }
        public string DownloadUrl { get; set; }
        public bool AppNeedsUpdate { get; set; }
    }

    public static class Updater
    {
        public static float Version = 1;
        private static readonly string UpdaterUrl = "https://dgagnonk.github.io/updaters/SimpleBackup.html";

        public async static Task<UpdateInfo> GetUpdateInfo()
        {
            string updateStr;

            using (WebClient client = new WebClient())
            {
                try
                {
                    updateStr = await client.DownloadStringTaskAsync(UpdaterUrl);
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }

            string[] updateParams = updateStr.Split('|');

            float.TryParse(updateParams[0], out float updaterVersion);

            UpdateInfo info = new UpdateInfo
            {
                Version = updaterVersion,
                DownloadUrl = updateParams[1],
                AppNeedsUpdate = Version < updaterVersion
            };

            return info;
        }
    }
}
