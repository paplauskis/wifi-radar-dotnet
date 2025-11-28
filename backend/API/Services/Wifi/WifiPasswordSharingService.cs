using API.Data.Repositories.Interfaces;
using API.Domain.Dto;
using API.Domain.Exceptions;
using API.Services.Interfaces.Wifi;

namespace API.Services.Wifi;

public class WifiPasswordSharingService : IWifiPasswordSharingService
{
    private readonly IWifiRepository _wifiRepository;

    public WifiPasswordSharingService(IWifiRepository wifiRepository)
    {
        _wifiRepository = wifiRepository;
    }

    public async Task<string> AddPasswordAsync(PasswordDto passwordDto)
    {
        if (passwordDto == null)
            throw new ArgumentNullException(nameof(passwordDto), "passwordDto is null");

        if (
            string.IsNullOrEmpty(passwordDto.City) || 
            string.IsNullOrEmpty(passwordDto.Street) || 
            passwordDto.BuildingNumber < 1)
        {
            throw new ArgumentException($"City ({passwordDto.City}), street ({passwordDto.Street}), or building number ({passwordDto.BuildingNumber}) values are invalid.");
        }

        if (string.IsNullOrWhiteSpace(passwordDto.Password) || passwordDto.Password.Length > 75)
            throw new ArgumentException($"Invalid password: \"{passwordDto.Password}\"", nameof(passwordDto.Password));

        await _wifiRepository.AddPasswordAsync(
            passwordDto.City,
            passwordDto.Street,
            passwordDto.BuildingNumber,
            passwordDto.Password);
        
        return passwordDto.Password;
    }

    public async Task<List<string>> GetPasswordsAsync(string city, string street, int buildingNumber)
    {
        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(street) || buildingNumber < 1)
        {
            throw new ArgumentNullException($"City ({city}), street ({street}), or building number ({buildingNumber}) values are invalid.");
        }

        var passwordsList = await 
            _wifiRepository.GetPasswordsByWifiAddressAsync(city, street, buildingNumber);

        if (passwordsList == null || passwordsList.Count == 0)
            throw new EmptyResponseException("");
        
        return passwordsList;
    }

    public async Task SharePasswordAsync(string wifiNetworkId, string password)
    {
        if (string.IsNullOrWhiteSpace(wifiNetworkId))
            throw new ArgumentException("Wifi network ID cannot be empty.", nameof(wifiNetworkId));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        await _wifiRepository.UpdatePasswordAsync(wifiNetworkId, password);
    }
}