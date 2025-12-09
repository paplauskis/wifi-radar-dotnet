using API.Domain.Dto;
using API.Domain.Models;

namespace API.Services.Interfaces.User;

public interface IUserFavoriteService
{
    Task<List<WifiNetworkDto>> GetUserFavoritesAsync(string userId);
    Task<WifiNetworkDto> AddUserFavoriteAsync(string userId, WifiNetworkDto wifi);
    Task DeleteUserFavoriteAsync(string userId, string city, string street, int buildingNumber);
}