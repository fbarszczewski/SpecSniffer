using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using MySql.Data.MySqlClient;
using SpecSniffer.Model.Spec;

namespace SpecSniffer.Model
{
    public class MysqlWorker
    {
        private static readonly string _connString =
            "SERVER= " +
            ";USERID= " +
            ";PASSWORD= " +
            ";DATABASE= specsniffer" +
            ";Connection Timeout=3;";

        private static readonly MySqlConnection StatusConn = new MySqlConnection(_connString);

        private readonly MySqlConnection _conn;
        private MySqlCommand _cmd;
        private MySqlDataReader _dr;

        public MysqlWorker()
        {
            _conn = new MySqlConnection(_connString);
        }

        public static bool DataBaseStatus { get; set; }


        public void SaveSpec(DeviceSpec spec, string comments, string rp, string reference, string license, string logType)
        {
                try
                {
                    _cmd = new MySqlCommand {Connection = _conn};


                    _cmd.CommandText = "INSERT INTO Devices " +
                                       "(manufacturer,serial,model,chassisType,ramSizeSum,ramSize,ramPN,ramSN,Cpu,hddSize,hddPN,hddSN," +
                                       "hddHealth,optical,netDevices,resolution,gpu,osName,osBuild,osLanguages,osSerial,osLicense,batteryPN,batteryHealth,batterySerial," +
                                       "batteryCharge,deviceList,comments,saveReference,rp,licenseLabel, logType) " +
                                       "VALUES (@manufacturer,@serial,@model,@chassisType,@ramSizeSum,@ramSize,@ramPN,@ramSN,@Cpu,@hddSize," +
                                       "@hddPN,@hddSN,@hddHealth,@optical,@netDevices,@resolution,@gpu,@osName,@osBuild,@osLanguages,@osSerial," +
                                       "@osLicense,@batteryPN,@batteryHealth,@batterySerial,@batteryCharge,@deviceList,@comments,@saveReference,@rp,@licenseLabel,@logType)";


                    _cmd.Parameters.AddWithValue("@model", spec.DeviceModel);
                    _cmd.Parameters.AddWithValue("@manufacturer", spec.Manufacturer);
                    _cmd.Parameters.AddWithValue("@serial", spec.DeviceSerial);
                    _cmd.Parameters.AddWithValue("@chassisType", spec.ChassisType);
                    _cmd.Parameters.AddWithValue("@ramSizeSum", spec.RamList.Sum(x => x.Size));
                    _cmd.Parameters.AddWithValue("@ramSize", string.Join("/", spec.RamList.Select(x => x.Size + "GB")));
                    _cmd.Parameters.AddWithValue("@ramPN", string.Join("/", spec.RamList.Select(x => x.PartNumber)));
                    _cmd.Parameters.AddWithValue("@ramSN", string.Join("/", spec.RamList.Select(x => x.Serial)));
                    _cmd.Parameters.AddWithValue("@Cpu",
                        spec.Cpu.Substring(0, WmiQuery.Cpu().LastIndexOf("@", StringComparison.Ordinal)).Trim());
                    _cmd.Parameters.AddWithValue("@hddSize",
                        string.Join(Environment.NewLine, spec.HddList.Where(x => x.Internal).Select(x => x.Size)));
                    _cmd.Parameters.AddWithValue("@hddPN",
                        string.Join(Environment.NewLine, spec.HddList.Where(x => x.Internal).Select(x => x.Model)));
                    _cmd.Parameters.AddWithValue("@hddSN",
                        string.Join(Environment.NewLine, spec.HddList.Where(x => x.Internal).Select(x => x.Serial)));
                    _cmd.Parameters.AddWithValue("@hddHealth",
                        string.Join(Environment.NewLine,
                            spec.HddList.Where(x => x.Internal).Select(x => x.Warning ? "Warning" : "OK")));
                    _cmd.Parameters.AddWithValue("@optical", string.Join("/", spec.OddList));

                    _cmd.Parameters.AddWithValue("@netDevices",
                        string.Join("/", spec.NetDevicesId.Select(x => $"[{x}]")));

                    _cmd.Parameters.AddWithValue("@resolution", spec.Resolution);
                    _cmd.Parameters.AddWithValue("@gpu", string.Join(Environment.NewLine, spec.GpuList));
                    _cmd.Parameters.AddWithValue("@osName", spec.OperatingSystem.Name);
                    _cmd.Parameters.AddWithValue("@osBuild", spec.OperatingSystem.BuildNumber);
                    _cmd.Parameters.AddWithValue("@osLanguages", spec.OperatingSystem.Languages);
                    _cmd.Parameters.AddWithValue("@osSerial", spec.OperatingSystem.SerialNumber);
                    _cmd.Parameters.AddWithValue("@osLicense", spec.OperatingSystem.LicenseKey);
                    _cmd.Parameters.AddWithValue("@batteryHealth",
                        string.Join("/", spec.Batteries.Select(x => $"{x.Health}%")));
                    _cmd.Parameters.AddWithValue("@batterySerial",
                        string.Join("/", spec.Batteries.Select(x => x.Serial)));
                    _cmd.Parameters.AddWithValue("@batteryCharge",
                        string.Join("/", spec.Batteries.Select(x => $"{x.PowerLeft} {x.ChargeRate}")));
                    _cmd.Parameters.AddWithValue("@batteryPN", string.Join("/", spec.Batteries.Select(x => x.Name)));
                    _cmd.Parameters.AddWithValue("@deviceList",
                        string.Join(Environment.NewLine, spec.Drivers.Select(x => $"{x.Caption} [{x.ErrorCode}]")));

                    _cmd.Parameters.AddWithValue("@comments", comments);
                    _cmd.Parameters.AddWithValue("@saveReference", reference);
                    _cmd.Parameters.AddWithValue("@rp", rp);
                    _cmd.Parameters.AddWithValue("@licenseLabel", license);
                    _cmd.Parameters.AddWithValue("@logType", logType);

                    _conn.Open();
                    _cmd.Prepare();
                    _cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    _conn.Close();
                    _cmd.Dispose();
                }

        }

        private void AddLicenseToSpec(string sn,string license)
        {

            try
            {
                _cmd = new MySqlCommand
                {
                    Connection = _conn,
                    //CommandText = $"UPDATE Devices SET cmarLicense={license} WHERE serial='{sn}' ORDER BY id DESC LIMIT 1"
                    CommandText = $"UPDATE Devices SET cmarLicense='{license}' WHERE serial='{sn}' ORDER BY id DESC LIMIT 1"
                };
                _cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _conn.Close();
            }
        }

        public void AddLicense(string newMar, string oldCoa, string licenseType, string deviceType, string rp,
            string reference, string model, string manufacturer, string serial, string cpu)
        {
            try
            {
                _cmd = new MySqlCommand
                {
                    Connection = _conn,
                    CommandText = "INSERT INTO Licenses " +
                                  "(newMar, OldCOA, licenseType, manufacturer, model, serial, deviceType, cpu, rp, reference) " +
                                  "VALUES (@newMar, @OldCOA, @licenseType, @manufacturer, @model, @serial, @deviceType, @cpu, @rp, @reference)"
                };



                _cmd.Parameters.AddWithValue("@newMar", newMar.Trim());
                _cmd.Parameters.AddWithValue("@OldCOA", oldCoa.Trim());
                _cmd.Parameters.AddWithValue("@licenseType", licenseType.Trim());
                _cmd.Parameters.AddWithValue("@manufacturer", manufacturer.Trim());
                _cmd.Parameters.AddWithValue("@model", model.Trim());
                _cmd.Parameters.AddWithValue("@serial", serial.Trim());
                _cmd.Parameters.AddWithValue("@deviceType", deviceType.Trim());
                _cmd.Parameters.AddWithValue("@cpu", cpu.Trim());
                _cmd.Parameters.AddWithValue("@rp", rp.Trim());
                _cmd.Parameters.AddWithValue("@reference", reference.Trim());

                _conn.Open();
                _cmd.Prepare();
                _cmd.ExecuteNonQuery();
                AddLicenseToSpec(serial,newMar);
                MessageBox.Show("License Saved :)");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                _conn.Close();
            }
        }

        public DataTable GetLastLogs()
        {
            var dataTable = new DataTable();
            try
            {
                _cmd = new MySqlCommand {Connection = _conn};

                _cmd.CommandText = "SELECT " +
                                   "saveReference as SO," +
                                   "rp, " +
                                   "serial," +
                                   "CONCAT(model,' ',cpu,'/',ramSizeSum,'GB/',REPLACE(hddSize,'\\r\\n','/'),'/',optical,'/',resolution,'/',LicenseLabel) as Description," +
                                   "DATE_FORMAT(TIMESTAMP(date,'1:00:00'),'%d/%m %T') as timeStamp," +
                                   "cmarLicense as CMAR," +
                                   " REPLACE(comments,'\\r\\n','/') as comments " +
                                   "FROM Devices WHERE visible='0'  ORDER BY id DESC LIMIT 12";

                _conn.Open();
                dataTable.Load(_cmd.ExecuteReader());
            }
            catch (Exception)
            {
                //ignore
            }
            finally
            {
                _conn.Close();
            }

            return dataTable;
        }



        private bool ExistInDatabase(string column, string table, string obj)
        {
            try
            {
                _cmd = new MySqlCommand
                {
                    Connection = _conn,
                    CommandText =
                        $"SELECT {column} FROM {table} WHERE {column} = @{column} AND visible='0' ORDER BY id DESC LIMIT 1"
                };

                _cmd.Parameters.AddWithValue($"@{column}", obj);
                _conn.Open();
                _cmd.Prepare();
                _dr = _cmd.ExecuteReader();
                if (_dr.Read())
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _conn.Close();
            }
        }

        public string ShowLastDescription()
        {
            if(ExistInDatabase("serial", "Devices", WmiQuery.SerialNumber()))
            {
                return GetDescription();
            }
            else
            {
                return "Not found.";
            }
        }

        private string GetDescription()
        {
            try
            {
                _cmd = new MySqlCommand {Connection = _conn};
                _cmd.CommandText = "SELECT " +
                                   "CONCAT(model,' ',cpu,'/',ramSizeSum,'GB/',REPLACE(hddSize,'\\r\\n','/'),'/',optical,'/',resolution,'\n',LicenseLabel) " +
                                   $"FROM Devices WHERE serial='{WmiQuery.SerialNumber()}' AND visible='0' ORDER BY id DESC LIMIT 1";



                _conn.Open();
                MySqlDataReader dr = _cmd.ExecuteReader();
                dr.Read();

                return dr.GetValue(0).ToString();

            }
            catch (Exception)
            {
                return "Couldn't get device's last description.";
            }
            finally
            {
                _conn.Close();
            }
        }

        public static bool DatabaseStatus()
        {
            var status = false;
            try
            {
                StatusConn.Open();
                status = true;
            }
            catch (Exception)
            {
                //ignored
            }
            finally
            {
                StatusConn?.Close();
            }

            return status;
        }
    }
}
