using System;
using System.Net;
using Telegram_Bot.SimpleJSON;

namespace Telegram_Bot
{
    public class GoogleAPI
    {
        public delegate void GResponse(object sendr, GParameterResponse e);
        public class GParameterResponse : EventArgs
        {
            public string lat;
            public string log;
        }

        public class GoogleApi
        {
            public string _token;
            public GoogleApi(string Token)
            {
                _token = Token;
            }

            public event GResponse GResponseRecieved;
            GParameterResponse e = new GParameterResponse();

            public void GetGoogleLocation(string adress)
            {
                while (true)
                {
                    using (WebClient webClient = new WebClient())
                    {
                        string Response = webClient.DownloadString("https://maps.googleapis.com/maps/api/geocode/json?address=" + adress + "&key=" + _token);
                        //if (Response.Length <= 200)
                        //    continue;
                        var N = JSON.Parse(Response);
                        foreach (JSONNode r in N["geometry"].AsArray)
                        {

                            e.lat = r["geometry"]["location"]["lat"];
                            e.log = r["geometry"]["location"]["lng"];

                        }
                    }
                    GResponseRecieved(this, e);
                }
            }
        }
    }
}
