using API.Domain.Exceptions;
using API.Services.Interfaces.Map;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/api/map")]
public class MapController : ControllerBase
{
    private readonly IMapService _mapService;

    public MapController(IMapService mapService)
    {
        _mapService = mapService;
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string city, [FromQuery] int? radius) //radius in meters
    {
        try
        {
            var wifis = await _mapService.Search(city, radius);
            return Ok(wifis);
        }
        catch (ArgumentNullException ane)
        {
            return BadRequest(ane.Message);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (EmptyResponseException)
        {
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Unexpected error occured: {e.Message}");
        }
    }

    [HttpGet("searchByIp")]
    public async Task<IActionResult> Search()
    {
        var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                 ?? HttpContext.Connection.RemoteIpAddress?.ToString();

        try
        {
            var wifis = await _mapService.SearchWifisFromNearestCity(ip);
            return Ok(wifis);
        }
        catch (ArgumentNullException ane)
        {
            return BadRequest(ane.Message);
        }
        catch (EmptyResponseException)
        {
            return NoContent();
        }
    }


    [HttpGet("coordinates")]
    public async Task<IActionResult> GetCoordinates(
        [FromQuery] string city,
        [FromQuery] string street,
        [FromQuery] int buildingNumber)
    {
        try
        {
            var dto = await _mapService.GetCoordinates(city, street, buildingNumber);
            return Ok(dto);
        }
        catch (EmptyResponseException e)
        {
            return NoContent();
        }
        catch (ArgumentNullException e)
        {
            return BadRequest("Could not get coordinates from this address" + e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}