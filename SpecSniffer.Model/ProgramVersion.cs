using System.Diagnostics;
using System.Windows;

namespace SpecSniffer.Model
{
    public class ProgramVersion
    {
        public static bool HasBeenChecked { get; set; }

        public static bool IsProgramUpdated(string current)
        {
            if (HasBeenChecked != true)
            {
                HasBeenChecked = true;
                var thisVer = "1.35";
                var currentVer = current;

                if (thisVer != currentVer)
                {

                    return false;
                }

                else return true;
            }

            return true;
        }
    }
}
