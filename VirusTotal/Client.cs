using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirusTotal
{
    internal class VTClient
    {
        static public RestClient GetClient()
        {
            var _apikey = Environment.GetEnvironmentVariable("VIRUSTOTAL_API_KEY") ?? "";
            var _client = new RestClient("https://www.virustotal.com/api/v3/");
            _client.AddDefaultHeader("x-apikey", _apikey);
            return _client;

        }


    }
}
