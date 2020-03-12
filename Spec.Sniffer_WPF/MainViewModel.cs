using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Spec.Sniffer_WPF.Camera;
using SpecSniffer.Model;
using SpecSniffer.Model.Spec;


namespace Spec.Sniffer_WPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            //_runDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location
            //    .Replace(@"\Spec.Sniffer_WPF.exe","").Trim();
            _runDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
            _runDirectory = _runDirectory.Substring(0, _runDirectory.LastIndexOf("\\", StringComparison.Ordinal));

            var specThread = new Thread(LoadSpec);
            specThread.Start();
            var driversThread = new Thread(LoadDriversList);
            driversThread.Start();
            var camThread = new Thread(LoadCamera);
            camThread.Start();
            WifiConnector.Connect();
            PreviousSettings = new LastSettings($"{_runDirectory}\\Resources\\Settings.xml");

            _deviceTimer.Tick += _deviceTimer_Tick;
            _deviceTimer.Interval = TimeSpan.FromSeconds(4);

            _statusTimer.Tick += _statusTimer_Tick;
            _statusTimer.Interval = TimeSpan.FromSeconds(2);
            _statusTimer.Start();

            _batteryTimer.Interval = TimeSpan.FromSeconds(2);
            _batteryTimer.Tick += Battery_Tick;
            _batteryTimer.Start();

        }




        #region Commands
        public ICommand ScreenCommand
        {
            get { return new RelayCommand(argument => OpenScreenTest()); }
        }

        public ICommand CameraCommand
        {
            get { return new RelayCommand(argument => CameraButton()); }
        }

        public ICommand AudioTestCommand
        {
            get { return new RelayCommand(argument => RunAudioTest()); }
        }

        public ICommand KeyboardCommand
        {
            get { return new RelayCommand(argument => KeyboardTest()); }
        }

        public ICommand DevmgmtCommand
        {
            get { return new RelayCommand(argument => Process.Start("devmgmt.msc")); }
        }

        public ICommand DiskmgmtCommand
        {
            get { return new RelayCommand(argument => Process.Start("diskmgmt.msc")); }
        }

        public ICommand HdTuneCommand
        {
            get { return new RelayCommand(argument => HdTune()); }
        }

        public ICommand ShowKeyCommand
        {
            get { return new RelayCommand(argument => ShowKey()); }
        }

        public ICommand OcctCommand
        {
            get { return new RelayCommand(argument => Occt()); }
        }
        public ICommand SpecTabCommand
        {
            get { return new RelayCommand(argument => SpecTabSelected = true); }
        }

        public ICommand DriversTabCommand
        {
            get { return new RelayCommand(argument => DriversTabSelected = true); }
        }

        public ICommand DiagTabCommand
        {
            get { return new RelayCommand(argument => DiagTabSelected = true); }
        }

        public ICommand SaveTabCommand
        {
            get { return new RelayCommand(argument => SaveTabSelected = true); }
        }

        public ICommand InstallDriversCommand
        {
            get { return new RelayCommand(argument => DriversFolder.InstallDrivers()); }
        }

        public ICommand WinUpdateCommand
        {
            get { return new RelayCommand(argument => Process.Start("ms-settings:windowsupdate-action")); }
        }

        public ICommand RunFileCommand
        {
            get { return new RelayCommand(argument => DriversFolder.RunFile()); }
        }

        #endregion
        private void LoadSpec()
        {
            Spec.Manufacturer = WmiQuery.Manufacturer();
            Spec.ChassisType = WmiQuery.ChassisType();
            Spec.DeviceModel = WmiQuery.Model();
            Spec.DeviceSerial = WmiQuery.SerialNumber();
            Spec.Cpu = WmiQuery.Cpu();
            Spec.RamList = WmiQuery.Ram();
            Spec.HddList = WmiQuery.Storages().Where(x => x.Internal).ToList();
            //gpu in SpecTabSelected
            //network device in SpecTabSelected
            Spec.OddList = WmiQuery.OpticalDrive();
            Spec.Resolution = $"{WmiQuery.Diagonal()} {WmiQuery.Resolution()}";
            Spec.OperatingSystem = WmiQuery.CurrentOs();
            SpecTabSelected = true;

            //for intel cpu
            if(Spec.Cpu.Contains("@"))
            {
                Spec.SpecSummary =
                $"{Spec.DeviceModel} " +
                $"{Spec.Cpu.Substring(0, Spec.Cpu.LastIndexOf("@", StringComparison.Ordinal))}/" +
                $"{Spec.RamList.Sum(x => x.Size)}GB/" +
                $"{string.Join("/", Spec.HddList.Where(x => x.Internal).Select(x => x.Size))}/" +
                $"{string.Join("/", Spec.OddList)}/" +
                $"{Spec.Resolution}";
            }
            // for others cpu
            else
            {
                Spec.SpecSummary =
                $"{Spec.DeviceModel} " +
                $"{Spec.Cpu}/" +
                $"{Spec.RamList.Sum(x => x.Size)}GB/" +
                $"{string.Join("/", Spec.HddList.Where(x => x.Internal).Select(x => x.Size))}/" +
                $"{string.Join("/", Spec.OddList)}/" +
                $"{Spec.Resolution}";
            }
           


        }




        #region Properties
        private string _runDirectory;
        private bool _specTabSelected;
        private bool _driversTabSelected;
        private bool _diagTabSelected;
        private bool _saveTabSelected;
        private bool _internetStatus;
        private bool _shareStatus;
        private bool _databaseStatus;
        private bool _usbStatus;

        private readonly DispatcherTimer _deviceTimer = new DispatcherTimer();
        private readonly DispatcherTimer _statusTimer = new DispatcherTimer();
        private IEnumerable<MediaInformation> _mediaDeviceList;
        private bool _camVisibility;
        private MediaInformation _selectedVideoDevice;

        private string _comments;
        private string _oldCoa;
        private string _deviceType;
        private DataTable _specDataTable;


        public DeviceSpec Spec { get; set; } = new DeviceSpec();
        public NetDrive DriversFolder { get; set; } = new NetDrive();

        public LastSettings PreviousSettings { get; set; } 



        public bool SpecTabSelected
        {
            get => _specTabSelected;
            set
            {
                if (value && _deviceTimer.IsEnabled) _deviceTimer.Stop();

                if (value&&Spec!=null)
                {
                    Spec.GpuList = WmiQuery.Gpu();
                    Spec.NetDevicesId = WmiQuery.NetDevices();
                }

                _specTabSelected = value;
                RaisePropertyChanged("SpecTabSelected");
            }
        }

        public bool DriversTabSelected
        {
            get => _driversTabSelected;
            set
            {

                if (value && !_deviceTimer.IsEnabled) _deviceTimer.Start();
                //select devices folder


                _driversTabSelected = value;
                RaisePropertyChanged("DriversTabSelected");
            }
        }

        public bool DiagTabSelected
        {
            get => _diagTabSelected;
            set
            {
                if (value && !_deviceTimer.IsEnabled) _deviceTimer.Start();

                _diagTabSelected = value;
                RaisePropertyChanged("DiagTabSelected");
            }
        }

        public bool SaveTabSelected
        {
            get => _saveTabSelected;
            set
            {
                if (value && _deviceTimer.IsEnabled) _deviceTimer.Stop();
                _saveTabSelected = value;
                RaisePropertyChanged("SaveTabSelected");
            }
        }


        public string ModelAndChassis => $"{Spec.DeviceModel} [{Spec.ChassisType}]";

        private readonly MysqlWorker _database = new MysqlWorker();

        public string Comments
        {
            get => _comments;
            set
            {
                _comments = value.Trim();
                RaisePropertyChanged("Comments");
            }

        }

        public string NewCmar { get; set; }

        public string OldCoa
        {
            get => string.IsNullOrEmpty(_oldCoa) ? "" : _oldCoa;
            set => _oldCoa = value.Trim();
        }

        public string DeviceType
        {
            get => string.IsNullOrEmpty(_deviceType) ? "" : _deviceType;
            set => _deviceType = value.Trim();
        }

        public DataTable SpecDataTable
        {
            get => _specDataTable;
            set
            {
                _specDataTable = value;
                RaisePropertyChanged("SpecDataTable");
            }
        }

        public IEnumerable<MediaInformation> MediaDeviceList
        {
            get => _mediaDeviceList;
            set
            {
                _mediaDeviceList = value;
                RaisePropertyChanged("MediaDeviceList");
            }
        }

        

        public bool CamVisibility
        {
            get => _camVisibility;
            set
            {
                _camVisibility = value;
                RaisePropertyChanged("CamVisibility");
            }
        }

        public MediaInformation SelectedVideoDevice
        {
            get => _selectedVideoDevice;

            set
            {
                _selectedVideoDevice = value;
                RaisePropertyChanged("SelectedVideoDevice");
            }
        }
        private readonly DispatcherTimer _batteryTimer = new DispatcherTimer();
        public bool InternetStatus
        {
            get => _internetStatus;
            set
            {
                //if (value && _database != null)
                //{
                //    if (ProgramVersion.HasBeenChecked == false)
                //    {
                //        if (!ProgramVersion.IsProgramUpdated(_database.GetCurrentVersion()))
                //        {
                //            MessageBox.Show("New version of Spec Sniffer is out.\n" +
                //                            "Click OK to open download page.\n\n" +
                //                            "Whats new:\n" +
                //                            $"{_database.GetChangeLog()}\n", 
                //                $"New version available.", MessageBoxButton.OK, MessageBoxImage.Information);

                //            Process.Start("https://www.dropbox.com/sh/svpay4c31hqat4i/AAD-QUtDT4DpKZ03Rlh4YTY5a?dl=0");
                //            MainWindow.CloseMainWindowNow();
                //        }

                //    }
                //}

                _internetStatus = value;
                RaisePropertyChanged("InternetStatus");


            }
        }

        public bool ShareStatus
        {
            get => _shareStatus;
            set
            {
                _shareStatus = value;
                RaisePropertyChanged("ShareStatus");
            }
        }

        public bool DatabaseStatus
        {
            get => _databaseStatus;
            set
            {
                _databaseStatus = value;
                RaisePropertyChanged("DatabaseStatus");
            }
        }

        public bool UsbStatus
        {
            get => _usbStatus;
            set
            {
                _usbStatus = value;
                RaisePropertyChanged("UsbStatus");
            }
        }




        #endregion





        #region DiagnosticTab

        private void LoadCamera()
        {
            MediaDeviceList = WebcamDevice.GetVideoDevices;
            SelectedVideoDevice = null;
            CamVisibility = true;
        }

        private void CameraButton()
        {
            if (SelectedVideoDevice == null)
            {
                CamVisibility = true;
                SelectedVideoDevice = MediaDeviceList.FirstOrDefault();
            }
            else
            {
                CamVisibility = false;
                SelectedVideoDevice = null;
            }
        }

        private void Battery_Tick(object sender, EventArgs e)
        {
            Spec.Batteries = WmiQuery.DeviceBatteries();
        }

        private void OpenScreenTest()
        {
            var lcdTest = new LcdTest();
            lcdTest.Show();
        }

        private void KeyboardTest()
        {
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = $"{_runDirectory}\\Resources\\KeyboardTest.exe";
                startInfo.Arguments = "-CS:1";
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HdTune()
        {
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = $"{_runDirectory}\\Resources\\HDTune.exe";
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowKey()
        {
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = $"{_runDirectory}\\Resources\\ShowKey.exe";
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Occt()
        {
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = $"{_runDirectory}\\Resources\\OCCT.exe";
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RunAudioTest()
        {
            var audioTest = new AudioIO(_runDirectory);
            audioTest.Show();
        }


      

        #endregion

        #region DriversTab

        private void _deviceTimer_Tick(object sender, EventArgs e)
        {
            LoadDriversList();
        }

        private void LoadDriversList()
        {
            if(Spec!=null)
                Spec.Drivers = WmiQuery.DriversList();
        }

        #endregion

        #region SaveTab


        public ICommand SoSaveCommand
        {
            get { return new RelayCommand(argument => SaveSpec("1")); }
        }
        public ICommand PoSaveCommand
        {
            get { return new RelayCommand(argument => SaveSpec("2")); }
        }
        public ICommand RmaSaveCommand
        {
            get { return new RelayCommand(argument => SaveSpec("3")); }
        }
        public ICommand OtherSaveCommand
        {
            get { return new RelayCommand(argument => SaveSpec("4")); }
        }

        public ICommand SaveLicenseCommand
        {
            get { return new RelayCommand(argument => SaveLicense()); }
        }

        private void SaveLicense()
        {
            if (CanSaveLicense())
            {
                _database.AddLicense(NewCmar, OldCoa, PreviousSettings.LicenseLabel, DeviceType, PreviousSettings.Rp,
                    PreviousSettings.Reference, Spec.DeviceModel, Spec.Manufacturer, Spec.DeviceSerial, Spec.Cpu);
                SpecDataTable = _database.GetLastLogs();
            }
        }



        private void SaveSpec(string saveType)
        {
                        var result = MessageBox.Show(
                        $"Last specification:" +
                        $"\n{_database.ShowLastDescription()}" +
                        $"\n\nDevice status check:" +
                        $"\n{Spec.DeviceStatusSummary(PreviousSettings.LicenseLabel)}" +
                        $"\n\n\n" +
                        $"Do You want to log this device?",
                        "Save Summary", MessageBoxButton.YesNo, MessageBoxImage.Information);


            if(result == MessageBoxResult.Yes)
            {
                _database.SaveSpec(Spec, PreviousSettings.Comments, PreviousSettings.Rp, PreviousSettings.Reference,
                PreviousSettings.LicenseLabel,saveType);
                PreviousSettings.WriteSettings();
                SpecDataTable = _database.GetLastLogs();
            }

        }



        private bool CanSaveLicense()
        {
            var errorList = new List<string>();
            var canSave = true;
            if (string.IsNullOrEmpty(PreviousSettings.Rp) || string.IsNullOrEmpty(PreviousSettings.Reference) ||
                string.IsNullOrEmpty(PreviousSettings.LicenseLabel))
            {
                MessageBox.Show("To save You must enter User, Reference and License.",
                    "Cannot save.", MessageBoxButton.OK, MessageBoxImage.Warning);
                canSave = false;
            }
            else
            {
                if (!PreviousSettings.LicenseLabel.Contains("CMAR"))
                    errorList.Add("Only Win10 Home & Pro CMAR can be reported.");

                if (string.IsNullOrEmpty(NewCmar))
                    errorList.Add("Enter new CMAR.");
                else if (!long.TryParse(NewCmar.Trim(), out var x) || NewCmar.Length <= 13)
                    errorList.Add("Invalid CMAR license.");


                if (string.IsNullOrEmpty(OldCoa))
                {
                    if (string.IsNullOrEmpty(Spec.DeviceModel)) errorList.Add("Enter Model.");
                    if (string.IsNullOrEmpty(Spec.Manufacturer)) errorList.Add("Enter Manufacturer.");
                    if (string.IsNullOrEmpty(Spec.DeviceSerial)) errorList.Add("Enter Serial.");
                    if (string.IsNullOrEmpty(DeviceType)) errorList.Add("Enter Device Type.");
                    if (string.IsNullOrEmpty(Spec.Cpu)) errorList.Add("Enter Cpu.");
                }
                else if (!long.TryParse(OldCoa.Trim(), out var x) || OldCoa.Length <= 13)
                {
                    errorList.Add("Invalid Old COA license");
                }


                if (errorList.Count > 0)
                {
                    var result = MessageBox.Show("Correct these warnings before save:\n\n" +
                                                 $"{string.Join("\n", errorList)}\n",
                        "Cant Save", MessageBoxButton.OK, MessageBoxImage.Warning);

                    canSave = false;
                }
            }

            return canSave;
        }

        #endregion


        #region Status

        private void _statusTimer_Tick(object sender, EventArgs e)
        {
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            InternetStatus = Network.CheckInternetConn();
            ShareStatus = NetDrive.NetDriveStatus();
            UsbStatus = Directory.Exists(_runDirectory);
        }

        #endregion



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
