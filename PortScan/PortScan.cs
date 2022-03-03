using Mercury.Plugin;
using System.Net;
using System.Net.Http.Headers;

namespace PortScan
{
    public class PortScan : IPlugin
    {
        public string Name { get; } = "PORTSCAN";
        private readonly string _key;
        public PortScan()
        {
            _key = Environment.GetEnvironmentVariable("VIEWDNS_KEY") ?? String.Empty;
        }
        public async Task<IServiceResult> QueryURL(string url)
        {
            if (Dns.GetHostAddresses(url).FirstOrDefault() == null)
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
                client.BaseAddress = new Uri($"https://api.viewdns.info/portscan/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage msg = await client.GetAsync($"?host={url}&apikey={_key}&output=json");
                if (msg.IsSuccessStatusCode)
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
    }
}