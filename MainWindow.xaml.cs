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
using System.Diagnostics;

namespace SimpleBackup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        DispatcherTimer runTimer;
        TimeSpan runTime;
        bool running = false;
        Configuration currentConfig;
        bool configChanged = false;

        public MainWindow()
        {
            InitializeComponent();
            Logger.LogAdded += new EventHandler(Logger_LogAdded);
            exLog.Expanded += ExLog_Expanded;
            exLog.Collapsed += ExLog_Collapsed;
            this.Closing += MainWindow_Closing;
            Logger.Log("SimpleBackup Activity Log", false);

            currentConfig = new Configuration();
            SetConfigChanges(currentConfig);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (configChanged)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Changes have been made to the app's configuration. Would you like to save your config before quitting?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                } else if (result == MessageBoxResult.Yes)
                {
                    SaveConfig();
                }
            }
        }

        private void SetConfigChanges(Configuration config)
        {
            chkMinimize.IsChecked = config.MinimizeToTray;
            txtMins.Text = config.BackupEveryMins.ToString();
            config.FilesToBackup.ForEach((string file) => lbFiles.Items.Add(file));
            config.BackupLocations.ForEach((string loc) => lbBackupLocs.Items.Add(loc));
            exLog.IsExpanded = config.LogIsExpanded;
        }

        private void SaveConfig()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                currentConfig.SaveConfig(sfd.FileName);
                configChanged = false;
            }
        }

        private void StartButtonEnabledHandler(bool? forcedValue = null)
        {
            if (forcedValue != null) btnStart.IsEnabled = forcedValue.GetValueOrDefault(); 
            if (lbFiles.Items.Count > 0 && lbBackupLocs.Items.Count > 0 && txtMins.Text.Length > 0) btnStart.IsEnabled = true;
            else btnStart.IsEnabled = false;
        }

        private void OnStartEnableDisable()
        {
            btnAddBackup.IsEnabled = !running;
            btnAddFile.IsEnabled = !running;
            btnOpenConfig.IsEnabled = !running;
            btnRemBackup.IsEnabled = !running;
            btnRemFile.IsEnabled = !running;
            btnSaveConfig.IsEnabled = !running;
            txtMins.IsEnabled = !running;
        }

        private void ExLog_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height -= 163;
        }

        private void ExLog_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height += 163;
        }

        private void Logger_LogAdded(object sender, EventArgs e)
        {
            txtConsole.Text += Logger.GetLastLog();
            txtConsole.ScrollToEnd();
            if (txtConsole.Text.Split(Environment.NewLine.ToCharArray()).Length > 50000)
            {
                Logger.Clear(ref txtConsole);
                Logger.Log("Log had over 50 000 lines. Clearing...");
            }
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
                configChanged = true;
            }

            StartButtonEnabledHandler();
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

            configChanged = true;
            StartButtonEnabledHandler();
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
                timer.Interval = new TimeSpan(0, 0, Convert.ToInt32(mins * 60));
                timer.Start();
                btnStart.Content = "Stop";
                running = true;
                lblTimeOn.Visibility = Visibility.Visible;
                Logger.Log("START backup loop");

                runTimer = new DispatcherTimer();
                runTimer.Tick += RunTimer_Tick;
                runTimer.Interval = new TimeSpan(0, 0, 1);
                runTime = TimeSpan.Zero;
                runTimer.Start();
                
            } 
            else
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                timer = null;
                btnStart.Content = "Start";
                running = false;

                lblTimeOn.Visibility = Visibility.Hidden;
                runTimer.Stop();
                runTimer.Tick -= RunTimer_Tick;
                runTimer = null;
                runTime = new TimeSpan();

                Logger.Log("STOP: Ran for " + lblTimeOn.Content);
            }

            OnStartEnableDisable();
            StartButtonEnabledHandler();
        }

        private void RunTimer_Tick(object sender, EventArgs e)
        {
            runTime = runTime.Add(new TimeSpan(0, 0, 1));
            lblTimeOn.Content = runTime.ToString();
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
                configChanged = true;
            }

            StartButtonEnabledHandler();
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

            configChanged = true;
            StartButtonEnabledHandler();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
        }

        private void btnOpenConfig_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                currentConfig.LoadConfig(ofd.FileName);
                SetConfigChanges(currentConfig);
                configChanged = false;
            }
        }

        private void txtMins_TextChanged(object sender, TextChangedEventArgs e)
        {
            float mins;
            bool validMins = float.TryParse(txtMins.Text, out mins);
            if (!validMins)
            {
                txtMins.Background = new SolidColorBrush(Colors.Salmon);
                StartButtonEnabledHandler(false);
            }
            else
            {
                txtMins.Background = new SolidColorBrush(Colors.White);
                configChanged = true;
                StartButtonEnabledHandler();
            }
        }
    }
}
