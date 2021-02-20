using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SimpleBackup
{
    public static class Logger
    {

        private static List<string> log = new List<string>();

        public static event EventHandler LogAdded;

        public static void Log(string message, bool showTimestamp = true)
        {
            string timestamp = "";
            if (showTimestamp) timestamp = string.Format("[{0}] ", DateTime.Now.ToString("yyyy/MM/dd - HH:mm"));

            log.Add(timestamp + message + Environment.NewLine);

            if (LogAdded != null)
                LogAdded(null, EventArgs.Empty);
        }

        public static void Clear(ref TextBox consoleBox)
        {
            log.Clear();
            consoleBox.Text = "";
        }

        public static string GetLastLog()
        {
            if (log.Count > 0)
                return log[log.Count - 1];
            else
                return null;
        }

        public static string GetLogAsString()
        {
            string str = "";
            foreach(string entry in log)
            {
                str += entry + Environment.NewLine;
            }
            return str;
        }
    }
}
