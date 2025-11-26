using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LapsAIApiGateWay.Services
{
    public class ServiceAClient : IServiceAClient
    {
        private readonly HttpClient _http;

        public ServiceAClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<string> GetWeatherAsync(CancellationToken cancellationToken = default)
        {
            // Calls ServiceA's WeatherForecast endpoint and returns raw JSON
            var response = await _http.GetAsync("/weatherforecast", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

       
    }
}
