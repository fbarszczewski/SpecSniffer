using System.Net;

namespace SpecSniffer.Model
{
    public class Network
    {
        public static bool CheckInternetConn()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}