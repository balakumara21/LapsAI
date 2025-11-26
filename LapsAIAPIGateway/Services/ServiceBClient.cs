using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LapsAIApiGateWay.Services
{
    public class ServiceBClient : IServiceBClient
    {
        private readonly HttpClient _http;

        public ServiceBClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> GetWeatherAsync(CancellationToken cancellationToken = default)
        {
            var response = await _http.GetAsync("/weatherforecast", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
