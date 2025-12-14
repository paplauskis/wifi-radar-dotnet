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
    
    //request can be checked out by pasting this JS code in browser console (replace <url> with current url):
    //fetch("http://<url>/api/map/search/offline?city=Kaunas", {
    //     headers: { Accept: "text/json" }
    //})
    //.then(res => {
    //     if (!res.ok) throw new Error("Request failed");
    //     return res.blob();
    //})
    //.then(blob => {
    //     const url = URL.createObjectURL(blob);
    //     const a = document.createElement("a");
    //     a.href = url;
    //     a.download = "wifis.json";
    //     document.body.appendChild(a);
    //     a.click();
    //     a.remove();
    //     URL.revokeObjectURL(url);
    //});
    // supports these Accept headers - text/json, text/xml
    [HttpGet("search/offline")]
    [Produces("text/json", "text/xml")]
    public async Task<IActionResult> SearchAndSaveToFile([FromQuery] string city, [FromQuery] int? radius)
    {
        try
        {
            var fileType = Request.Headers.Accept.First();
            var file = await _mapService.SearchAndSaveToFile(city, fileType, radius);
            return File(file.FileContent, file.HttpHeader, file.FileName);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (InvalidOperationException ioe)
        {
            return BadRequest(ioe.Message);
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