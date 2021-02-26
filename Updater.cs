using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

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
        public static float Version = 1.0f;
        private static readonly string UpdaterUrl = "https://dgagnonk.github.io/updaters/SimpleBackup.html";

        public async static Task<UpdateInfo> GetUpdateInfo()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            string updateStr;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "SimpleBackup");
                updateStr = await client.GetStringAsync(UpdaterUrl);
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
