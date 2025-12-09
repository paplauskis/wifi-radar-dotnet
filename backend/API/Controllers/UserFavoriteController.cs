using API.Domain.Dto;
using API.Domain.Exceptions;
using API.Exceptions;
using API.Services.Interfaces.User;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/api/user")]
public class UserFavoriteController : ControllerBase
{
    private readonly IUserFavoriteService _userFavoriteService;
    
    public UserFavoriteController(IUserFavoriteService userFavoriteService)
    {
        _userFavoriteService = userFavoriteService;
    }

    [HttpGet("{userId}/favorites")]
    public async Task<IActionResult> GetFavorites([FromRoute] string userId)
    {
        try
        {
            var favorites = await _userFavoriteService.GetUserFavoritesAsync(userId);
            return favorites.Count == 0 ? NoContent() : Ok(favorites);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (InvalidDataException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Unexpected server error occurred");
        }
    }

    [HttpPost("{userId}/favorites")]
    public async Task<IActionResult> AddFavorite([FromRoute] string userId, [FromBody] WifiNetworkDto dto)
    {
        try
        {
            var addedFavorite = await _userFavoriteService.AddUserFavoriteAsync(userId, dto);
            return Ok(addedFavorite);
        }
        catch (InvalidInputException e)
        {
            return BadRequest(e.Message);
        }
        catch (WifiNetworkAlreadyExistsException e)
        {
            return Conflict(e.Message);
        }
        catch (UserNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ArgumentException e)
        {
            return BadRequest($"WifiNetworkDto parameter data is not valid; {e.Message}");
        }
        catch (Exception)
        {
            return StatusCode(500, "Unexpected server error occurred");
        }
    }
    
    [HttpDelete("{userId}/favorites")]
    public async Task<IActionResult> DeleteFavorite(
        [FromRoute] string userId,
        [FromQuery] string city, 
        [FromQuery] string street, 
        [FromQuery] int buildingNumber)
    {
        try
        { 
            await _userFavoriteService.DeleteUserFavoriteAsync(userId, city, street, buildingNumber);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (UserNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Unexpected server error occurred");
        }
    }
}
