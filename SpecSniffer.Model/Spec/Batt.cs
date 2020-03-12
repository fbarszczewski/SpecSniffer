using System;

namespace SpecSniffer.Model.Spec
{
    public class Batt
    {
        /// <summary>
        ///     Battery name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Battery unique value.
        /// </summary>
        public string Serial { get; set; }

        /// <summary>
        ///     Battery capacity left  in percents. Known to not working in HP devices.
        /// </summary>
        public uint Health => CalculateBatteryHealth();

        /// <summary>
        ///     Remaining capacity in percents.
        /// </summary>
        public int PowerLeft { get; set; }

        /// <summary>
        ///     True if device is connected to charger.
        /// </summary>
        public bool? Charging { get; set; }

        /// <summary>
        ///     Charging/discharging rate in milliwatts.
        /// </summary>
        public string ChargeRate { get; set; }

        /// <summary>
        ///     Design capacity of the battery in milliwatt-hours.
        /// </summary>
        public uint DesignCapacity { get; set; }

        /// <summary>
        ///     Full charge capacity of the battery in milliwatt-hours.
        ///     Comparison of the value to the DesignCapacity property determines when the battery requires replacement.
        /// </summary>
        public uint CurrentCapacity { get; set; }

        public string HealthAndName => $"{Health}% - {Name}";

        private uint CalculateBatteryHealth()
        {
            uint health = 0;
            try
            {
                if (CurrentCapacity != 0)
                {
                    health = CurrentCapacity * 100 / DesignCapacity;
                    health = health > 100 ? 100 : health;
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return health > 100 ? 100 : health;
        }
    }
}
