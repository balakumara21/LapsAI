using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LapsAIApiGateWay.Controllers
{
    [Authorize]
    [ApiController]
    [Route("call")]
    public class ProxyController : ControllerBase
    {
        private readonly Services.IServiceAClient _serviceA;
        private readonly Services.IServiceBClient _serviceB;

        public ProxyController(Services.IServiceAClient serviceA, Services.IServiceBClient serviceB)
        {
            _serviceA = serviceA;
            _serviceB = serviceB;
        }

        [HttpGet("servicea")]
        public async Task<IActionResult> CallServiceA()
        {
            var json = await _serviceA.GetWeatherAsync();
            return Content(json, "application/json");
        }

        [HttpGet("serviceb")]
        public async Task<IActionResult> CallServiceB()
        {
            var json = await _serviceB.GetWeatherAsync();
            return Content(json, "application/json");
        }
    }
}
