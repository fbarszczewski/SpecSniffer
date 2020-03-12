namespace SpecSniffer.Model.Spec
{
    public class Os
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => _name = value.Replace("Microsoft","").Trim();
        }

        public string BuildNumber { get; set; }
        public string Languages { get; set; }
        public bool IsPortable { get; set; }
        /// <summary>
        /// Operating system product serial identification number.
        /// </summary>
        public string SerialNumber { get; set; }
        public string LicenseKey { get; set; }
    }
}
