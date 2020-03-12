using System;
using System.Collections.Generic;
using SimpleWifi;

namespace SpecSniffer.Model
{
    public static class WifiConnector
    {



        public static void Connect()
        {
            try
            {
                var _wlan = new Wifi();
                IEnumerable<AccessPoint> apList = _wlan.GetAccessPoints();

                foreach (var ap in apList)
                    if (ap.Name == "Unit5_1")
                    {
                        var authRequest = new AuthRequest(ap);
                        if (authRequest.IsUsernameRequired)
                            authRequest.Username = "Unit5_1";

                        authRequest.Password = "GHS@543%torun";
                        ap.ConnectAsync(authRequest, true);
                        break;
                    }
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}
