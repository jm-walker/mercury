using Mercury.Plugin;
using RestSharp;
using System.Text.Json;

namespace VirusTotal
{
    public class VTReport : IPlugin
    {
        public string Name => "VTREPORT";

        public async Task<IServiceResult> QueryURL(string url)
        {
            UriHostNameType hostType = Uri.CheckHostName(url);

            bool isIP = (hostType == UriHostNameType.IPv4 || hostType == UriHostNameType.IPv6);

            var client = VTClient.GetClient();
            var req = isIP ? new RestRequest("ip_addresses/{id}") : new RestRequest("domain/{id}");

            req.AddParameter("id", url);
            RestResponse result = await client.GetAsync(req);

            if( result == null ||! result.IsSuccessful)
            {
                return new ServiceResult()
                {
                    ResultMessage = $"Failed - {(int)(result?.StatusCode ?? 0)} {result?.StatusDescription}",
                    Status = ResultStatus.FAILURE,
                    Service = Name
                };
            }

            return new ServiceResult()
            {
                ResultMessage = "SUCCESS",
                Result = JsonSerializer.Deserialize<dynamic>(result.Content ?? ""),
                Status = ResultStatus.SUCCESS,
                Service = Name
            };

        }
    }
}