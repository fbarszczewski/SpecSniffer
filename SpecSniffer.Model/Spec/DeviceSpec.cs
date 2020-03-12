using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SpecSniffer.Model.Spec
{
    public class DeviceSpec : INotifyPropertyChanged
    {
        private List<Batt> _batteries;
        private string _chassisType;
        private string _cpu;
        private List<Driver> _drivers;
        private string _deviceSerial;
        private List<string> _gpuList;
        private List<Storage> _hddList;
        private string _manufacturer;
        private string _model;
        private List<uint> _netDevicesId;
        private List<string> _oddList;
        private Os _operatingSystem;
        private List<Memory> _ramList;
        private string _resolution;
        private string _specSummary;

        public string DeviceModel
        {
            get => _model;
            set
            {
                _model = value;
                RaisePropertyChanged("DeviceModel");
                RaisePropertyChanged("ModelAndChassis");
                RaisePropertyChanged("SpecSummary");
            }
        }

        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                _manufacturer = value;
                RaisePropertyChanged("Manufacturer");
            }
        }

        public string ChassisType
        {
            get => _chassisType;
            set
            {
                _chassisType = value;
                RaisePropertyChanged("ChassisType");
                RaisePropertyChanged("ModelAndChassis");
            }
        }

        public string DeviceSerial
        {
            get => _deviceSerial;
            set
            {
                _deviceSerial = value;
                RaisePropertyChanged("DeviceSerial");
            }
        }

        public string Cpu
        {
            get => _cpu;
            set
            {
                _cpu = value;
                RaisePropertyChanged("Cpu");
                RaisePropertyChanged("SpecSummary");
            }
        }

        public List<Memory> RamList
        {
            get => _ramList;
            set
            {
                _ramList = value;
                RaisePropertyChanged("RamList");
                RaisePropertyChanged("SpecSummary");
            }
        }

        public List<Storage> HddList
        {
            get => _hddList;
            set
            {
                _hddList = value;
                RaisePropertyChanged("HddList");
                RaisePropertyChanged("SpecSummary");
            }
        }

        public List<string> GpuList
        {
            get => _gpuList;
            set
            {
                _gpuList = value;
                RaisePropertyChanged("GpuList");
            }
        }

        public List<string> OddList
        {
            get => _oddList;
            set
            {
                _oddList = value;
                RaisePropertyChanged("OddList");
                RaisePropertyChanged("SpecSummary");
            }
        }

        public string Resolution
        {
            get => _resolution;
            set
            {
                _resolution = value;
                RaisePropertyChanged("Resolution");
                RaisePropertyChanged("SpecSummary");
            }
        }

        public Os OperatingSystem
        {
            get => _operatingSystem;
            set
            {
                _operatingSystem = value;
                RaisePropertyChanged("OperatingSystem");
            }
        }

        public List<Driver> Drivers
        {
            get => _drivers;
            set
            {
                _drivers = value;
                RaisePropertyChanged("Drivers");
                RaisePropertyChanged("FprOpacity");
                RaisePropertyChanged("CamOpacity");
                RaisePropertyChanged("MissingDrivers");
                RaisePropertyChanged("DriversErrors");
            }
        }
        public List<Driver> DriversErrors 
        { 
            get
            {
                if (Drivers != null)
                    return Drivers.Where(x => x.ErrorCode != 0 && x.ErrorCode != 28 && !x.Caption.Contains("PS/2")).ToList();
                else
                    return null;
            }
        }

        public List<Driver> MissingDrivers
        {
            get
            {
                if (Drivers != null)
                    return Drivers.Where(x => x.ErrorCode == 28).ToList();
                else
                    return null;
            }
        }
        public List<uint> NetDevicesId
        {
            get => _netDevicesId;
            set
            {
                _netDevicesId = value;
                RaisePropertyChanged("NetDevicesId");
                RaisePropertyChanged("EthOpacity");
                RaisePropertyChanged("BluetoothOpacity");
                RaisePropertyChanged("WlanOpacity");
                RaisePropertyChanged("WwanOpacity");
            }
        }
        public bool EthOpacity => NetDevicesId != null ? NetDevicesId.Any(x => x == 1 || x == 14) : false;
        public bool BluetoothOpacity => NetDevicesId != null ? NetDevicesId.Any(x => x == 10) : false;
        public bool WlanOpacity => NetDevicesId != null ? NetDevicesId.Any(x => x == 1 || x == 9) : false;
        public bool WwanOpacity => NetDevicesId != null ? NetDevicesId.Any(x => x == 8) : false;
        public bool CamOpacity => Drivers != null
            ? Drivers.Any(x => x.PnpClass.ToLower().Contains("camera"))
            : false;

        public bool FprOpacity => Drivers != null
            ? Drivers.Any(x => x.PnpClass.Contains("Biometric")) ? true :
            Drivers.Any(x => x.Caption.Contains("w/ Fingerprint"))
            : false;

        public List<Batt> Batteries
        {
            get => _batteries;
            set
            {
                _batteries = value;
                RaisePropertyChanged("Batteries");
            }
        }



        public string SpecSummary
        {
            get => _specSummary;
            set
            { 
                _specSummary = value;
                RaisePropertyChanged("SpecSummary");
            }
        }




        public string DeviceStatusSummary(string licenseLabel)
        {
            List<string> errors=new List<string>();



            //Look for not installed GPU driver
            
            if(GpuList.Select(x=>x.Contains("Microsoft")).First())
            {
                errors.Add($"Missing GPU driver.");
            }


            //missing device drivers
            if(MissingDrivers.Count>0)
            {
                errors.Add($"Missing {MissingDrivers.Count} device driver's.");
            }

            //drivers errors
            if(DriversErrors.Count>0)
            {
                errors.Add($"{DriversErrors.Count} drivers not working properly.");
            }

            //pl language
            if(!OperatingSystem.Languages.Contains("PL"))
            {
                errors.Add("Polish language is not included.");
            }

            //license check
            if(licenseLabel.Contains("OEM")&& string.IsNullOrWhiteSpace(OperatingSystem.LicenseKey))
            {
                errors.Add("OS license key has not been found.");
            }

            //battery health 
            if(Batteries.Select(x => x.Health < 40 && x.Name != "No battery").First())
            {
                errors.Add("Battery health below 40%.");
            }


            //battery swollen detection
            if(Manufacturer!="HEWLETT-PACKARD"||Manufacturer!="HP")
            {

                foreach(var battery in Batteries)
                {
                    if(battery.Health==100)
                    {
                        if(Manufacturer!="HEWLETT-PACKARD"||Manufacturer!="HP")
                        {
                            if(battery.Charging==true)
                            {
                                if(battery.PowerLeft!=100)
                                {
                                    if(battery.ChargeRate=="+0")
                                    {
                                    errors.Add("Detected possibly swollen battery!");
                                    }
                                }
                            }
                            else
                            {
                            errors.Add("AC adapter not connected. Couldn't check if battery is swollen.");
                            }
                        }

                    }
                    else if(battery.PowerLeft<95 && battery.ChargeRate=="+0")
                    {
                        errors.Add("Battery is not charging despite it isn't fully charged.");
                    }

                }
            }




            if(errors.Count>0)
            {
                return string.Join(Environment.NewLine,errors);
            }
            else
            {
                return "All Good ;)";

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
