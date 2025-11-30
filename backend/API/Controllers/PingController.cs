using API.Services.Interfaces.Status;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        private readonly IOverpassApiStatusService _overpassApiStatusService;

        public PingController(IOverpassApiStatusService overpassApiStatusService)
        {
            _overpassApiStatusService = overpassApiStatusService;
        }
        
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "backend-ok", time = DateTime.Now });

        // sometimes Overpass API has a lot of load, because of that
        // requests may fail, this endpoint checks if it is stable
        [HttpGet("overpass-api")]
        public async Task<IActionResult> GetOverpassApiStatus()
        {
            var statusData = await _overpassApiStatusService.CheckStatus();
            return statusData.IsStable 
                ? Ok(new { time = DateTime.UtcNow, isStable = true, statusData.Responses }) 
                : StatusCode(503, new { time = DateTime.UtcNow, isStable = false, statusData.Responses });
        }
    }
}
