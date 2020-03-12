namespace SpecSniffer.Model.Spec
{
    public class Driver
    {
        /// <summary>
        /// Driver description.
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// Driver type eg. USB
        /// </summary>
        public string PnpClass { get; set; }
        public uint ErrorCode { get; set; }
        public string ErrorMessage
        {
            get
            {
                switch (ErrorCode)
                {
                    case 0:
                        return "This device is working properly.";
                    case 1:
                        return "This device is not configured correctly.";
                    case 2:
                        return "Windows cannot load the driver for this device.";
                    case 3:
                        return
                            "The driver for this device might be corrupted, or your system may be running low on memory or other resources. ";
                    case 4:
                        return
                            "This device is not working properly. One of its drivers or your registry might be corrupted.";
                    case 5:
                        return "The driver for this device needs a resource that Windows cannot manage.";
                    case 6:
                        return "The boot configuration for this device conflicts with other devices. ";
                    case 7:
                        return "Cannot filter.";
                    case 8:
                        return "The driver loader for the device is missing.";
                    case 9:
                        return
                            "This device is not working properly because the controlling firmware is reporting the resources for the device incorrectly.";
                    case 10:
                        return "This device cannot start.";
                    case 11:
                        return "This device failed.";
                    case 12:
                        return "This device cannot find enough free resources that it can use. ";
                    case 13:
                        return "Windows cannot verify this device's resources.";
                    case 14:
                        return "This device cannot work properly until you restart your computer.";
                    case 15:
                        return
                            "This device is not working properly because there is probably a re-enumeration problem.";
                    case 16:
                        return "Windows cannot identify all the resources this device uses.";
                    case 17:
                        return "This device is asking for an unknown resource type. ";
                    case 18:
                        return "Reinstall the drivers for this device.";
                    case 19:
                        return "Failure using the VxD loader.";
                    case 20:
                        return "Your registry might be corrupted.";
                    case 21:
                        return
                            "System failure: Try changing the driver for this device. If that does not work, see your hardware documentation. Windows is removing this device.";
                    case 22:
                        return "This device is disabled.";
                    case 23:
                        return
                            "System failure: Try changing the driver for this device. If that doesn't work, see your hardware documentation.";
                    case 24:
                        return
                            "This device is not present, is not working properly, or does not have all its drivers installed.";
                    case 25:
                        return "Windows is still setting up this device.";
                    case 26:
                        return "Windows is still setting up this device.";
                    case 27:
                        return "This device does not have valid log configuration.";
                    case 28:
                        return "The drivers for this device are not installed.";
                    case 29:
                        return
                            "This device is disabled because the firmware of the device did not give it the required resources.";
                    case 30:
                        return "This device is using an Interrupt Request (IRQ) resource that another device is using.";
                    case 31:
                        return
                            "This device is not working properly because Windows cannot load the drivers required for this device.";

                    default:
                        return "...";
                }
            }
        }

        public string Status
        {
            get
            {
                switch (ErrorCode)
                {
                    case 0:
                        return "Fine";
                    default:
                        return "Warning";
                }
            }
        }

    }
}