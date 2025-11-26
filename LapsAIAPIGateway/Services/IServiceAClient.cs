using System.Threading;
using System.Threading.Tasks;

namespace LapsAIApiGateWay.Services
{
    public interface IServiceAClient
    {
        Task<string> GetWeatherAsync(CancellationToken cancellationToken = default);
    }
}
