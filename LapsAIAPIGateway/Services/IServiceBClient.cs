using System.Threading;
using System.Threading.Tasks;

namespace LapsAIApiGateWay.Services
{
    public interface IServiceBClient
    {
        Task<string> GetWeatherAsync(CancellationToken cancellationToken = default);
    }
}
