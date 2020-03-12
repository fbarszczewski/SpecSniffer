namespace SpecSniffer.Model.Spec
{
    public class Storage
    {
        private string _model;
        private string _serial;

        public string Model
        {
            get => _model;
            set => _model = value.Trim();
        }

        public string Serial
        {
            get => _serial;
            set => _serial = value.Trim();
        }

        /// <summary>
        ///     The media type of the physical disk (HDD,SSD).
        /// </summary>
        public DiskType StorageType { get; set; }

        /// <summary>
        ///     Type of disk (NVMe, RAID,SATA etc.)
        /// </summary>
        public DiskBus BusType { get; set; }

        public string Size { get; set; }

        /// <summary>
        ///     Storage SMART status.
        /// </summary>
        public bool Warning { get; set; } = false;

        /// <summary>
        ///     External or internal driver (USB or internal)
        /// </summary>
        public bool Internal { get; set; }
        public string PnpDevice { get; set; }

    }

    public enum DiskType : ushort
    {
        GB,
        HDD = 3,
        SSD,
        SCM
    }

    public enum DiskBus : ushort
    {
        Unknown,
        SCSI,
        ATAPI,
        ATA,
        IEEE1394,
        SSA,
        FibreChannel,
        USB,
        RAID,
        iSCSI,
        SAS,
        MMC,
        MAX,
        FileBackedVirtual,
        StorageSpaces,
        NVMe,
        MicrosoftReserved
    }
}
