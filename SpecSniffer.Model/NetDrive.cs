using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SpecSniffer.Model
{
    public class NetDrive : INotifyPropertyChanged
    {
        private static readonly string _netLetter = "S:";
        private static bool _isNetUseRunning;
        private IEnumerable<string> _files;
        private IEnumerable<string> _folders;
        private readonly DispatcherTimer _netShareTimer;
        private string _selectedFile;
        private string _selectedFolder;

        public NetDrive()
        {
            _netShareTimer = new DispatcherTimer();
            _netShareTimer.Tick += _netShareTimer_Tick;
            _netShareTimer.Interval = TimeSpan.FromSeconds(1);
            _netShareTimer.Start();
        }

        public IEnumerable<string> Folders
        {
            get => _folders;
            set
            {
                _folders = value;
                RaisePropertyChanged("Folders");
            }
        }

        public string SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                if (value != null)
                    try
                    {
                        Files = Directory.GetFiles($"{_netLetter}\\{value}")
                            .Select(x => x.Substring(x.LastIndexOf(@"\", StringComparison.Ordinal) + 1))
                            .Where(x=>x.Contains(".bat")==false);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                RaisePropertyChanged("SelectedFolder");
            }
        }

        public IEnumerable<string> Files
        {
            get => _files;
            set
            {
                _files = value;
                RaisePropertyChanged("Files");
            }
        }

        public string SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;

                RaisePropertyChanged("SelectedFile");
            }
        }

        private void _netShareTimer_Tick(object sender, EventArgs e)
        {
            ConnectNetShare();

            if (Directory.Exists(_netLetter))
            {
                SetFolders();
                _netShareTimer.Stop();
            }
        }




        public static bool NetDriveStatus()
        {
            return Directory.Exists(_netLetter);
        }

        public void SetFolders()
        {
            try
            {
                Folders = Directory.GetDirectories(_netLetter).Select(x => x.Replace(_netLetter, "").Trim());
                SetSelectedFolder();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void SetSelectedFolder()
        {
            try
            {
                foreach (var folder in Folders)
                    if (WmiQuery.Model().ToLower().Contains(folder.ToLower()))
                    {
                        SelectedFolder = folder;
                        break;
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void RunFile()
        {
            if (SelectedFile != null)
                try
                {
                    var thr = new Thread(RunSelectedFile);
                    thr.Start();
                }
                catch (Exception e)
                {
                    if (e.HResult != -2147467259) //if not aborted by user
                        MessageBox.Show(e.Message);
                }
        }

        private void RunSelectedFile()
        {
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = $@"{_netLetter}\{SelectedFolder}\{SelectedFile}";
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.Start();
                proc.WaitForExit();
                proc.Dispose();
            }
            catch (Exception)
            {
                //ignore
            }

        }
        public void InstallDrivers()
        {
            try
            {
                if (File.Exists($@"{_netLetter}\{SelectedFolder}\InstallDrivers.bat"))
                {
                    var thr = new Thread(RunInfIntaller);
                    thr.Start();
                }
                else
                {
                    MessageBox.Show($"InstallDrivers.bat not found for {SelectedFolder}");
                }
            }
            catch (Exception e)
            {
                if (e.HResult != -2147467259) //if not aborted by user
                    MessageBox.Show(e.Message);
            }
        }

        private void RunInfIntaller()
        {

            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = $@"{_netLetter}\{SelectedFolder}\InstallDrivers.bat";
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.Start();
                proc.WaitForExit();
                proc.Dispose();
            }
            catch (Exception)
            {
                //ignore
            }
        }

        private void ConnectNetShare()
        {
            if (Network.CheckInternetConn() && !Directory.Exists(_netLetter) && !_isNetUseRunning)
            {
                var netThread = new Thread(NetShare);
                netThread.Start();
            }
        }

        private void NetShare()
        {
            var cmd = new Process();
            cmd.StartInfo.FileName = "net.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.Arguments = $@"use {_netLetter} \\172.16.6.20\SpecSniffer\drivers /u:serwer snrqvx198 ";
            cmd.Start();
            _isNetUseRunning = true;
            cmd.WaitForExit();
            cmd.Dispose();
        }

        public static void RemoveNetShare()
        {
            if (Directory.Exists(_netLetter))
            {
                var cmd = new Process();
                cmd.StartInfo.FileName = "net.exe";
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.Arguments = $@"use {_netLetter} /delete";
                cmd.Start();
                cmd.WaitForExit();
                cmd.Dispose();
            }
        }

        #region INotify Property handler

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}