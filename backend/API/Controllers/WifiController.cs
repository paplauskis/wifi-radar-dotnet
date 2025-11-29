using API.Domain.Dto;
using API.Domain.Exceptions;
using API.Services.Interfaces.Wifi;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/api/wifi")]
public class WifiController : ControllerBase
{
    private readonly IWifiReviewService _wifiReviewService;
    private readonly IWifiPasswordSharingService _wifiPasswordSharingService;

    public WifiController(IWifiReviewService wifiReviewService, IWifiPasswordSharingService wifiPasswordSharingService)
    {
        _wifiReviewService = wifiReviewService;
        _wifiPasswordSharingService = wifiPasswordSharingService;
    }

    [HttpGet("reviews")]
    public async Task<IActionResult> GetWifiReviews(
        [FromQuery] string city, 
        [FromQuery] string street, 
        [FromQuery] int buildingNumber)
    {
        try
        {
            var reviews = await _wifiReviewService.GetReviewsAsync(city, street, buildingNumber);
            return Ok(reviews);
        }
        catch (EmptyResponseException)
        {
            return NoContent();
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Unexpected server error occurred: " + e.Message);
        }
    }

    [HttpPost("reviews")]
    public async Task<IActionResult> AddWifiReview([FromBody] WifiReviewDto wifiReviewDto)
    {
        try
        {
            var review = await _wifiReviewService.AddReviewAsync(wifiReviewDto);
            return Ok(review);
        }
        catch (ArgumentNullException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Unexpected server error occurred");
        }
    }

    [HttpGet("passwords")]
    public async Task<IActionResult> GetPasswords(
        [FromQuery] string city, 
        [FromQuery] string street, 
        [FromQuery] int buildingNumber)
    {
        try
        {
            var passwords = await _wifiPasswordSharingService.GetPasswordsAsync(city, street, buildingNumber);
            return Ok(passwords);
        }
        catch (EmptyResponseException)
        {
            return NoContent();
        }
        catch (ArgumentNullException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Unexpected server error occurred");
        }
    }

    [HttpPost("passwords")]
    public async Task<IActionResult> AddPassword([FromBody] PasswordDto passwordDto)
    {
        try
        {
            var password = await _wifiPasswordSharingService.AddPasswordAsync(passwordDto);
            return Ok(password);
        }
        catch (ArgumentNullException e)
        {
            return BadRequest(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Unexpected server error occurred");
        }
    }
}
