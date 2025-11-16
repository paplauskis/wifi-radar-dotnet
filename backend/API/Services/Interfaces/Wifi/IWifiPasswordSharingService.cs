using API.Domain.Dto;

namespace API.Services.Interfaces.Wifi;

public interface IWifiPasswordSharingService
{
    Task<string> AddPasswordAsync(PasswordDto passwordDto);
    Task<List<string>> GetPasswordsAsync(string city, string street, int buildingNumber);
}