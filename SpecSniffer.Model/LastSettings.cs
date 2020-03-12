using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml;

namespace SpecSniffer.Model
{
    public class LastSettings :INotifyPropertyChanged
    {
        private readonly XmlDocument _settingsFile = new XmlDocument();
        private readonly string _filePath;
        private string _rp;
        private string _reference;
        private string _licenseLabel;
        private string _comments;

        public LastSettings(string filePath)
        {
            _filePath =filePath;
            LoadSettings();
        }

        public string Rp
        {
            get => _rp;
            set
            {
                _rp = value.Trim();
                RaisePropertyChanged("Rp");
            }
        }

        public string Reference
        {
            get => _reference;
            set
            {
                _reference = value.Trim();
                RaisePropertyChanged("Reference");
            }
        }

        public string LicenseLabel
        {
            get => _licenseLabel;
            set
            {
                _licenseLabel = value;
                RaisePropertyChanged("LicenseLabel");
            }
        }

        public string Comments
        {
            get => _comments;
            set
            {
                _comments = value;
                RaisePropertyChanged("Comments");
            }
        }

        public void LoadSettings()
        {
            try
            {
                _settingsFile.Load(_filePath);

                var settingsNode = _settingsFile.DocumentElement.SelectSingleNode("/Settings");


                Rp = settingsNode.Attributes["LastRp"].Value;
                Reference = settingsNode.Attributes["LastReference"].Value;
                LicenseLabel = settingsNode.Attributes["LastLicense"].Value;
                Comments=settingsNode.Attributes["Comments"].Value;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Settings file not found.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read settings error.\n{ex.Message}");
            }
        }

        public void WriteSettings()
        {
            try
            {
                var settingsNode = _settingsFile.DocumentElement.SelectSingleNode("/Settings");

                settingsNode.Attributes["LastRp"].Value = Rp;
                settingsNode.Attributes["LastReference"].Value = Reference;
                settingsNode.Attributes["LastLicense"].Value = LicenseLabel;
                settingsNode.Attributes["Comments"].Value = Comments;

                _settingsFile.Save(_filePath);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Could not save settings. Pen drive missing.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save settings error.\n{ex.Message}");
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
