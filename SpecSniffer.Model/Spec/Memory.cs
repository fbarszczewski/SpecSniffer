namespace SpecSniffer.Model.Spec
{
    public class Memory
    {
        private string _partNumber;
        private string _serial;
        private string _clockSpeed;

        public string PartNumber
        {
            get => _partNumber;
            set => _partNumber = value.Trim();
        }

        public string Serial
        {
            get => _serial;
            set => _serial = value.Trim();
        }

        /// <summary>
        ///     Total capacity of the physical memory, in GB.
        /// </summary>
        public ushort Size { get; set; }
        /// <summary>
        /// Total capacity of the physical memory with 'GB' suffix
        /// </summary>
        public string SizeGb { get => $"{Size}GB"; } 


        /// <summary>
        ///     Label of the socket or circuit board that holds the memory.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        ///     The configured clock speed of the memory device, in MHz, or 0, if the speed is unknown.
        /// </summary>
        public string ClockSpeed
        {
            get
            {
                return _clockSpeed;
            }
            set
            {
                _clockSpeed = $"{value}MHz";
            }
        }

        public uint Millivolts { get; set; }


        /// <summary>
        ///     Location, size and speed
        /// </summary>
        public string RamInfo => $"{Size}GB {ClockSpeed} {Volts()}";


        private string Volts()
        {
            double volts = (double)Millivolts / 1000;
            return volts==0? null: $"{volts}v";
        }

    }
}