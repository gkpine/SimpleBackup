using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using SimpleBackup;
using System.Windows.Threading;
using System.Windows.Forms;

namespace SimpleBackup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        bool running = false;

        public MainWindow()
        {
            InitializeComponent();
            Logger.LogAdded += new EventHandler(Logger_LogAdded);
            Logger.Log("SimpleBackup Activity Log", false);
        }

        private void Logger_LogAdded(object sender, EventArgs e)
        {
            txtConsole.Text += Logger.GetLastLog();
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                foreach(string filename in ofd.FileNames)
                {
                    lbFiles.Items.Add(filename);
                    Logger.Log("ADD file " + filename);
                }
            }
        }

        private void btnRemFile_Click(object sender, RoutedEventArgs e)
        {
            if (lbFiles.SelectedIndex < 0) return;

            object[] selectedItems = lbFiles.SelectedItems.Cast<object>().ToArray();
            foreach(object selected in selectedItems)
            {
                lbFiles.Items.Remove(selected);
                Logger.Log("REMOVE file " + selected.ToString());
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!running)
            {
                float mins;
                bool validMins = float.TryParse(txtMins.Text, out mins);
                if (!validMins)
                {
                    System.Windows.MessageBox.Show("Minute value must be a whole number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = new TimeSpan(0, 0, Convert.ToInt32(mins / 60));
                timer.Start();
                btnStart.Content = "Stop";
                running = true;
            } 
            else
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer = null;
                btnStart.Content = "Start";
                running = false;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            FileManager.DoBackup(lbFiles.Items.OfType<string>().ToArray(), lbBackupLocs.Items.OfType<string>().ToArray());
        }

        private void btnAddBackup_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lbBackupLocs.Items.Add(fbd.SelectedPath);
                Logger.Log("ADD backup folder " + fbd.SelectedPath);
            }
        }

        private void btnRemBackup_Click(object sender, RoutedEventArgs e)
        {
            if (lbBackupLocs.SelectedIndex < 0) return;

            object[] selectedItems = lbBackupLocs.SelectedItems.Cast<object>().ToArray();
            foreach (object selected in selectedItems)
            {
                lbBackupLocs.Items.Remove(selected);
                Logger.Log("REMOVE backup folder " + selected.ToString());
            }
        }
    }
}
