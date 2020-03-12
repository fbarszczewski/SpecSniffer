using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using SpecSniffer.Model.Spec;

namespace SpecSniffer.Model
{
    public static class WmiQuery
    {
        private static Os _currentOs;
        private static List<Memory> _ram;
        private static List<Batt> _batteries;
        private static string _cpu;
        private static string _serialNumber;
        private static string _model;
        private static string _partNumber;
        private static string _chassisType;
        private static string _manufacturer;
        /// <summary>
        ///     Chassis types of device.
        /// </summary>
        public static string ChassisType()
        {
            if (_chassisType == null)
            {
                var output = "";
                ushort[] chassisArr = null;
                var chassisList = new List<string>();
                try
                {
                    foreach (var query in new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT ChassisTypes " +
                        "FROM Win32_SystemEnclosure").Get())
                    {
                        chassisArr = (ushort[]) query["ChassisTypes"];
                        output = string.Join("/", chassisArr);
                    }

                    foreach (Chassis chassis in chassisArr) chassisList.Add(chassis.ToString());
                }
                catch (Exception)
                {
                    chassisList.Add("error");
                }

                _chassisType = string.Join("/", chassisList);
            }

            return _chassisType;
        }

        public static string Manufacturer()
        {
            if (_manufacturer == null)
                _manufacturer = WmiQueryStringReturn("root\\CIMV2", "Win32_ComputerSystem", "Manufacturer").ToUpper();
            return _manufacturer;
        }

        public static string PartNumber()
        {
            if (_partNumber == null)
                _partNumber = WmiQueryStringReturn("root\\CIMV2", "Win32_ComputerSystemProduct", "Version");
            return _partNumber;
        }

        public static string Model()
        {
            if (_model == null)
            {
                if (Manufacturer().Contains("LENOVO"))
                    _model = PartNumber();
                else
                    _model = WmiQueryStringReturn("root\\CIMV2", "Win32_ComputerSystem", "Model");

                var modelSb = new StringBuilder(_model);

                if (Manufacturer().Contains("LENOVO"))
                {
                    modelSb.Replace("ThinkPad", "");
                    modelSb.Replace("ThinkCentre", "");
                }
                else if (Manufacturer().Contains("DELL"))
                {
                    modelSb.Replace("Workstation", "");
                    modelSb.Replace("Precision", "");
                    modelSb.Replace("Latitude", "");
                    modelSb.Replace("OptiPlex", "");
                    modelSb.Replace("non-vPro", "");
                    modelSb.Replace("Inspiron", "");
                    modelSb.Replace("Vostro", "");
                    modelSb.Replace("AIO", "");
                    modelSb.Replace("Tower", "TWR");
                }
                else if (Manufacturer().Contains("HEWLETT-PACKARD"))
                {
                    modelSb.Replace("All-in-One", " AiO ");
                    modelSb.Replace("Workstation", "");
                    modelSb.Replace("EliteBook", "");
                    modelSb.Replace("Precision", "");
                    modelSb.Replace("EliteDesk", "");
                    modelSb.Replace("Notebook", "");
                    modelSb.Replace("ProBook", "");
                    modelSb.Replace("Spectre", "");
                    modelSb.Replace("Compaq", "");
                    modelSb.Replace("COMPAQ", "");
                    modelSb.Replace("Elite", "");
                    modelSb.Replace("Folio", "");
                    modelSb.Replace("Pro", "");
                    modelSb.Replace("HP", "");
                    modelSb.Replace("PC", "");
                }
                else if (Manufacturer().Contains("FUJITSU"))
                {
                    modelSb.Replace("LIFEBOOK", "");
                    modelSb.Replace("ESPRIMO", "");
                }

                _model = Regex.Replace(modelSb.ToString(), @"\s+", " ").Trim();
            }

            return _model;
        }

        public static string SerialNumber()
        {
            if (_serialNumber == null)
                _serialNumber = Manufacturer().Contains("LENOVO")
                    ? $"S1{WmiQueryStringReturn("root\\CIMV2", "Win32_ComputerSystem", "Model")}" +
                      $"{WmiQueryStringReturn("root\\CIMV2", "Win32_SystemEnclosure", "SerialNumber")}"
                    : WmiQueryStringReturn("root\\CIMV2", "Win32_SystemEnclosure", "SerialNumber");

            return _serialNumber;
        }

        public static string Cpu()
        {
            if (_cpu == null)
            {
                var cpuTrim = new StringBuilder(
                    WmiQueryStringReturn("root\\CIMV2", "Win32_Processor", "Name"));

                cpuTrim.Replace("Intel(R) Core(TM)", "");
                cpuTrim.Replace("Intel(R) Xeon(R)", "");
                cpuTrim.Replace("Intel(R) Atom(TM)", "");
                cpuTrim.Replace("Intel(R) Pentium(R)", "");
                cpuTrim.Replace("CPU", "");


                _cpu = Regex.Replace(cpuTrim.ToString(), @"\s+", " ").Trim();
            }

            return _cpu;
        }

        public static List<Memory> Ram()
        {
            if (_ram == null)
            {
                _ram = new List<Memory>();

                try
                {
                    foreach (var queryObj in new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT PartNumber,SerialNumber,DeviceLocator,ConfiguredClockSpeed,Capacity,ConfiguredVoltage " +
                        "FROM Win32_PhysicalMemory").Get())
                    {
                        //_ram.Add(new Memory
                        //{
                        //    PartNumber = (string) queryObj["PartNumber"],
                        //    Serial = (string) queryObj["SerialNumber"],
                        //    Location = (string) queryObj["DeviceLocator"],
                        //    ClockSpeed = (uint) queryObj["ConfiguredClockSpeed"],
                        //    Size = (ushort) ((ulong) queryObj["Capacity"] / (1024 * 1024 * 1024)),
                        //    Millivolts = (uint)queryObj["ConfiguredVoltage"]
                        //});

                        var memory = new Memory();
                        try
                        {
                            memory.PartNumber = (string) queryObj["PartNumber"];
                        }
                        catch (Exception)
                        {
                            memory.PartNumber = "noData";
                        }

                        try
                        {
                            memory.Serial = (string) queryObj["SerialNumber"];
                        }
                        catch (Exception)
                        {
                            memory.Serial = "noData";
                        }

                        try
                        {
                            memory.Location = (string) queryObj["DeviceLocator"];
                        }
                        catch (Exception)
                        {
                            memory.Location = "noData";
                        }

                        try
                        {
                            memory.ClockSpeed = queryObj["ConfiguredClockSpeed"].ToString();
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            memory.Size = (ushort) ((ulong) queryObj["Capacity"] / (1024 * 1024 * 1024));
                        }
                        catch (Exception)
                        {
                            memory.Size = 0;
                        }

                        try
                        {
                            memory.Millivolts = (uint) queryObj["ConfiguredVoltage"];
                        }
                        catch (Exception)
                        {
                            memory.Millivolts = 0;
                        }

                        _ram.Add(memory);
                    }
                }
                catch (Exception)
                {
                    _ram.Add(new Memory
                    {
                        PartNumber = "error",
                        Serial = "error",
                        Location = "error",
                        Size = 0
                    });
                }
            }
            return _ram;
        }

        public static List<Storage> Storages()
        {
            var storages = new List<Storage>();
            var diskDrive = new ManagementObjectSearcher(
                "root\\CIMV2",
                "SELECT Size,Model,MediaType,SerialNumber,PNPDeviceID FROM Win32_DiskDrive");

            try
            {
                foreach (var queryObj in diskDrive.Get())
                    storages.Add(new Storage
                    {
                        Model = queryObj["Model"].ToString().Trim(),
                        Serial = queryObj["SerialNumber"].ToString().Trim(),
                        Internal = (string)queryObj["MediaType"] == "Fixed hard disk media" ? true : false,
                        PnpDevice = queryObj["PNPDeviceID"].ToString().Trim()
                    }); ;


                diskDrive.Scope = new ManagementScope("root\\Microsoft\\Windows\\Storage");
                diskDrive.Query = new ObjectQuery("SELECT BusType,MediaType,Size,SerialNumber FROM MSFT_PhysicalDisk");

                foreach (var queryObj in diskDrive.Get())
                {
                    foreach (var hdd in storages)
                    {
                        if(hdd.Serial.Contains(queryObj["SerialNumber"].ToString()))
                        {
                            hdd.BusType = (DiskBus)queryObj["BusType"];
                            hdd.StorageType = (DiskType)queryObj["MediaType"];
                            hdd.Size = ((int)((ulong)queryObj["Size"] / (1000 * 1000 * 1000))).ToString() + (DiskType)queryObj["MediaType"];
                        }

                    }

                }
            }
            catch (Exception)
            {
                storages.Add(new Storage
                {
                    Model = "error",
                    Serial = "error",
                    Size = "error",
                    Internal = false
                });
            }

            try
            {
                diskDrive.Scope = new ManagementScope("\\root\\wmi");
                diskDrive.Query = new ObjectQuery("SELECT VendorSpecific, InstanceName FROM MSStorageDriver_FailurePredictData");


                foreach (ManagementObject query in diskDrive.Get())
                {

                    foreach (Storage hdd in storages)
                    {
                        if(query["InstanceName"].ToString().ToUpper().Contains(hdd.PnpDevice.ToUpper()))
                        {
                            var bytes = (byte[])query.Properties["VendorSpecific"].Value;

                            for (var i = 0; i < 25; ++i)
                                try
                                {
                                    int id = bytes[i * 12 + 2];
                                    hdd.Warning = false;
                                    //check  Reallocation event count, Current pending sector count, Reallocated sector count,
                                    if (id == 196 || id == 197 || id == 5)
                                    {
                                        var data = BitConverter.ToInt32(bytes, i * 12 + 7);
                                        if (data != 0)
                                        {
                                            hdd.Warning = true;
                                            break;
                                        }

                                    }
                                }
                                catch (Exception)
                                {
                                    hdd.Warning = true;
                                }
                        }

                        
                    }

                    

                }
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show("HDD Status not loaded.\n" +
                //    "No admin privileges.");
            }


            return storages;
        }

        public static List<string> OpticalDrive()
        {
            List<string> oddList = new List<string>();

            try
            {
                foreach (var queryObj in new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT MediaType " +
                    "FROM Win32_CDROMDrive").Get())
                        oddList.Add((string)queryObj["MediaType"] == "DVD Writer"?"RW": (string)queryObj["MediaType"]);
            }
            catch (Exception)
            {
                
            }

            return oddList;
        }

        public static string Diagonal()
        {
            double verticalSize = 0;
            double horizontalSize = 0;
            var roundedDiagonal = "";
            var diagonalMap = new List<double>
                {10.1, 11.6, 12, 12.5, 13.3, 14, 15.6, 17.3, 18, 19, 20, 20.1, 21, 21.3, 22, 22.2, 23, 24, 26, 27};

            try
            {
                var searcher =
                    new ManagementObjectSearcher("root\\WMI",
                        "SELECT MaxHorizontalImageSize, MaxVerticalImageSize  FROM WmiMonitorBasicDisplayParams");

                foreach (ManagementObject queryObj in searcher.Get())
                    if (queryObj["MaxHorizontalImageSize"] != null)
                    {
                        verticalSize = Convert.ToDouble(queryObj["MaxVerticalImageSize"]) / 2.54;
                        horizontalSize = Convert.ToDouble(queryObj["MaxHorizontalImageSize"]) / 2.54;
                        break;
                    }

                var diagonal = Math.Sqrt(verticalSize * verticalSize + horizontalSize * horizontalSize);
                roundedDiagonal = diagonalMap.Select(n => new {n, distance = Math.Abs(n - diagonal)})
                    .OrderBy(p => p.distance)
                    .First().n.ToString();
            }
            catch (Exception)
            {
                roundedDiagonal = "error";
            }

            return $@"{roundedDiagonal}""";
        }

        public static string Resolution()
        {
            var resolution = "";
            var _resolutionMap = new Dictionary<string, string>
            {
                {"1280x1024", "SXGA"},
                {"1360x768", "HD"},
                {"1366x768", "HD"},
                {"1600x900", "HD+"},
                {"1920x1080", "FHD"},   
                {"1280x800", "WXGA"},
                {"1280x768", "WXGA"},
                {"1280x720", "WXGA"},
                {"1440x900", "WXGA"},
                {"1680x1050", "WSXGA"},
                {"1920x1200", "WUXGA"},
                {"1152x864", "XGA+"},
                {"1024x768", "XGA"},
                {"1024x600", "WSVGA"},
                {"800x600", "SVGA"},
                {"2560x1440", "WQHD"},
                {"3840x2160", "UHD"},
                {"4096x2160", "UHD"},
                {"2560×1600", "WQXGA"}
            };

            try
            {
                var searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT CurrentHorizontalResolution, CurrentVerticalResolution FROM Win32_VideoController");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    resolution = $"{queryObj["CurrentHorizontalResolution"]}x{queryObj["CurrentVerticalResolution"]}";
                    if(resolution=="x")
                        continue;
                    else
                        break;

                }
            }
            catch (Exception)
            {
                resolution = "error";
            }

            foreach (var resName in _resolutionMap.Where(resName => resName.Key == resolution))
            {
                resolution = resName.Value;
                break;
            }

            return resolution;
        }

        public static List<string> Gpu()
        {
            List<string> gpuList = new List<string>();
            //return WmiQueryStringReturn("root\\CIMV2", "Win32_VideoController", "Caption");

            try
            {
                foreach (var queryObj in new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT Caption " +
                    "FROM Win32_VideoController").Get())
                        gpuList.Add((string) queryObj["Caption"]);
            }
            catch (Exception)
            {
                
            }
            return gpuList;
        }

        public static Os CurrentOs()
        {
            if (_currentOs == null)
            {
                _currentOs = new Os();
                try
                {
                    foreach (var query in new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT Caption,BuildNumber, PortableOperatingSystem, MUILanguages,SerialNumber " +
                        "FROM Win32_OperatingSystem").Get())
                    {
                        _currentOs.Name = (string) query["Caption"];
                        _currentOs.BuildNumber = (string) query["BuildNumber"];
                        _currentOs.IsPortable = (bool) query["PortableOperatingSystem"];
                        _currentOs.Languages = string.Join(" ",
                            ((string[]) query["MUILanguages"]).Select(s => s = s.Remove(0, 3)).ToArray());
                        _currentOs.SerialNumber = (string) query["SerialNumber"];
                    }
                }
                catch (Exception)
                {
                    _currentOs.Name = "error";
                    _currentOs.BuildNumber = "error";
                    _currentOs.IsPortable = false;
                    _currentOs.Languages = "error";
                    _currentOs.SerialNumber = "error";
                }

                _currentOs.LicenseKey =
                    WmiQueryStringReturn("root\\CIMV2", "SoftwareLicensingService", "OA3xOriginalProductKey");
            }

            return _currentOs;
        }

        public static List<Driver> DriversList()
        {
            var deviceList = new List<Driver>();
            try
            {
                foreach (var queryObj in new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT Caption,ConfigManagerErrorCode,PNPClass " +
                    "FROM Win32_PnPEntity").Get())
                    deviceList.Add(
                        new Driver
                        {
                            Caption = string.IsNullOrWhiteSpace((string) queryObj["Caption"])
                                ? "No name"
                                : (string) queryObj["Caption"],
                            ErrorCode = (uint) queryObj["ConfigManagerErrorCode"],
                            PnpClass = string.IsNullOrWhiteSpace((string) queryObj["PNPClass"])
                                ? "No name"
                                : (string) queryObj["PNPClass"]
                        });
            }
            catch (Exception)
            {
                // ignored
            }


            return deviceList;
        }

        public static List<uint> NetDevices()
        {
            var netIdList = new List<uint>();
            try
            {
                foreach (var queryObj in new ManagementObjectSearcher("root\\StandardCimv2",
                    "SELECT NdisPhysicalMedium " +
                    "FROM MSFT_NetAdapter").Get())
                    netIdList.Add((uint) queryObj["NdisPhysicalMedium"]);
            }
            catch (Exception)
            {
                //ignored
            }

            return netIdList;
        }

        public static List<Batt> DeviceBatteries()
        {
            var batterySearcher = new ManagementObjectSearcher(
                "root\\CIMV2",
                "SELECT Name, DeviceID, EstimatedChargeRemaining FROM Win32_Battery");
            var battCount = 0;

            _batteries = new List<Batt>();

            if (batterySearcher.Get().Count > 0)
            {
                try
                {
                    foreach (var win32Battery in batterySearcher.Get())
                    {
                        var battery = new Batt();
                        try
                        {
                            battery.Name = string.IsNullOrEmpty((string) win32Battery["Name"])
                                ? "no name"
                                : win32Battery["Name"].ToString().Trim();
                        }
                        catch (Exception)
                        {
                            battery.Name = "no data";
                        }

                        try
                        {
                            battery.Serial = string.IsNullOrEmpty((string) win32Battery["DeviceID"])
                                ? "no id"
                                : win32Battery["DeviceID"].ToString().Trim();
                        }
                        catch (Exception)
                        {
                            battery.Serial = "no data";
                        }

                        try
                        {
                            battery.PowerLeft = (int)win32Battery["EstimatedChargeRemaining"];
                        }
                        catch (Exception)
                        {
                            battery.PowerLeft = -1;
                        }

                        _batteries?.Add(battery);
                    }
                }
                catch (Exception)
                {
                }

                battCount = 0;
                batterySearcher.Scope = new ManagementScope(@"root\WMI");
                batterySearcher.Query =
                    new ObjectQuery(@"SELECT Charging, Discharging, ChargeRate, DischargeRate FROM BatteryStatus");
                try
                {
                    foreach (var batteryStatus in batterySearcher.Get())
                    {
                        try
                        {
                            _batteries[battCount].Charging =
                                (bool) batteryStatus["Charging"] ? true : false;
                        }
                        catch (Exception)
                        {
                            _batteries[battCount].Charging = null;
                        }

                        try
                        {
                            if (_batteries[battCount].Charging == true)
                                _batteries[battCount].ChargeRate = $"+{batteryStatus["ChargeRate"]}";
                            else if (_batteries[battCount].Charging == false)
                                _batteries[battCount].ChargeRate = $"-{batteryStatus["DischargeRate"]}";
                            else
                                _batteries[battCount].ChargeRate = "0";
                        }
                        catch (Exception)
                        {
                            _batteries[battCount].ChargeRate = "null";
                        }


                        battCount++;
                    }
                }
                catch (Exception)
                {
                }

                battCount = 0;
                batterySearcher.Query = new ObjectQuery(@"SELECT FullChargedCapacity FROM BatteryFullChargedCapacity");
                try
                {
                    foreach (var batteryFullChargedCapacity in batterySearcher.Get())
                    {
                        _batteries[battCount].CurrentCapacity =
                            (uint) batteryFullChargedCapacity["FullChargedCapacity"];
                        battCount++;
                    }
                }
                catch (Exception)
                {
                }

                battCount = 0;
                batterySearcher.Query = new ObjectQuery(@"SELECT DesignedCapacity FROM BatteryStaticData");
                try
                {
                    foreach (var DesignedCapacity in batterySearcher.Get())
                    {
                        _batteries[battCount].DesignCapacity =
                            (uint) DesignedCapacity["DesignedCapacity"];
                        battCount++;
                    }
                }
                catch (Exception)
                {
                }
            }
            else
            {
                _batteries.Add(new Batt {Name = "No battery"});
            }

            return _batteries;
        }


        /// <summary>
        ///     Concatenate query results into single string.
        /// </summary>
        private static string WmiQueryStringReturn(string scopeNamespace, string scopeClass, string scopeProperty)
        {
            var propertyValue = new List<string>();

            try
            {
                foreach (var queryObj in new ManagementObjectSearcher(scopeNamespace,
                    $"SELECT {scopeProperty} " +
                    $"FROM {scopeClass}").Get())
                    propertyValue.Add(queryObj[scopeProperty].ToString().Trim());
            }
            catch (Exception)
            {
                propertyValue.Add("error");
            }

            return string.Join("/", propertyValue);
        }
    }

    public enum Chassis : ushort
    {
        Other = 1,
        Unknown,
        Desktop,
        LowProfileDesktop,
        PizzaBox,
        MiniTower,
        Tower,
        Portable,
        Laptop,
        Notebook,
        HandHeld,
        DockingStation,
        AiO,
        SubNotebook,
        SpaceSaving,
        LunchBox,
        MainSystemChassis,
        ExpansionChassis,
        SubChassis,
        BusExpansionChassis,
        PeripheralChassis,
        StorageChassis,
        RackMountChassis,
        SealedCasePc
    }
}
