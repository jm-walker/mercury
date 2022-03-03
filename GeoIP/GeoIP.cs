using Mercury.Plugin;
using System.Net;
using System.Net.Http.Headers;

namespace GeoIP
{
    public class GeoIP : IPlugin
    {
        public string Name { get; } = "GEOIP";
        private string _key;
        public GeoIP()
        {
            _key = Environment.GetEnvironmentVariable("GEOIP_KEY") ?? String.Empty;
        }
        public async Task<IServiceResult> QueryURL(string url)
        {
            string key = "";
            IPAddress? address = GetIPAddress(url);
            if(address == null)
            {
                return new ServiceResult()
                {
                    ResultMessage = $"Could not resolve '{url}'",
                    Service = Name,
                    Status = ResultStatus.FAILURE
                };
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"https://api.ipdata.co/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage msg = await client.GetAsync($"{address}?api-key={_key}");
                if(msg.IsSuccessStatusCode)
                {
                    string json = await msg.Content.ReadAsStringAsync();
                    dynamic? details = System.Text.Json.JsonSerializer.Deserialize<dynamic>(json);

                    return new ServiceResult()
                    {
                        Result = details,
                        Status = ResultStatus.SUCCESS,
                        ResultMessage = "Success",
                        Service = Name
                    };
                }
                else
                {
                    return new ServiceResult()
                    {
                        Result = await msg.Content.ReadAsStringAsync(),
                        Status = ResultStatus.FAILURE,
                        ResultMessage = $"Http Failure Code = {(int)msg.StatusCode} - {msg.StatusCode}",
                        Service = Name
                    };
                }

            }
        }

        private IPAddress? GetIPAddress(string name)
        {
            IPAddress[] ipaddress = Dns.GetHostAddresses(name);

            return ipaddress.FirstOrDefault();
        }
    }
}