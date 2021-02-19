using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleBackup;

namespace SimpleBackup
{
    public static class FileManager
    {
        public static void DoBackup(string[] files, string[] destinations)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HHmmss");
            foreach(string filepath in files)
            {
                FileInfo fi = new FileInfo(filepath);
                foreach (string dest in destinations)
                {
                    string newPath = string.Format("{0}\\{1}_{2}", dest, timestamp, fi.Name);
                    try
                    {
                        if (File.Exists(newPath)) continue;
                        File.Copy(filepath, newPath);
                        Logger.Log(string.Format("BACKUP file {0} to {1}", fi.Name, newPath));
                    }
                    catch (Exception ex)
                    {
                        throw new IOException(ex.Message);
                    }
                }
            }
        }
    }
}
