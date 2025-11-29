using API.Domain.Dto;
using MongoDB.Bson;

namespace Tests.Helpers;

public static class WifiNetworkDtoHelper
{
    public static IEnumerable<object[]> InvalidWifiNetworkDtos()
    {
        return new List<object[]>
        {
            new object[]
            {
                new WifiNetworkDto
                {
                    City = "Kaunas",
                    Name = "City Hotels Algirdas",
                    Street = "Algirdo g.",
                    BuildingNumber = 24,
                    PostalCode = 12345,
                    IsFree = true,
                    Password = new string('b', 111)
                }
            },
            new object[]
            {
                new WifiNetworkDto
                {
                    City = "Vilnius",
                    Name = "Tech Park",
                    Street = "Antakalnio g.",
                    BuildingNumber = 12,
                    PostalCode = 5432166,
                    IsFree = false,
                    Password = "secure123"
                }
            },
            new object[]
            {
                new WifiNetworkDto
                {
                    City = null,
                    Name = "Tech Park",
                    Street = "Antakalnio g.",
                    BuildingNumber = 12,
                    PostalCode = 54321,
                    IsFree = false,
                    Password = "secure123"
                }
            },
            new object[]
            {
                new WifiNetworkDto
                {
                    City = "Vilnius",
                    Name = "  ",
                    Street = "",
                    BuildingNumber = 12,
                    PostalCode = 54321,
                    IsFree = false,
                    Password = "secure123"
                }
            },
            new object[]
            {
                new WifiNetworkDto
                {
                    City = "Vilnius",
                    Name = "Tech Park",
                    Street = null,
                    BuildingNumber = 12,
                    IsFree = false,
                }
            },
            new object[]
            {
                new WifiNetworkDto
                {
                    City = "Vilnius",
                    Name = "Tech Park",
                    Street = null,
                    BuildingNumber = 0,
                    IsFree = false,
                }
            },
            new object[]
            {
                new WifiNetworkDto
                {
                    City = "Vilnius",
                    Name = "Tech Park",
                    Street = "null",
                    BuildingNumber = -3,
                    IsFree = false,
                }
            },
            new object[]
            {
                new WifiNetworkDto
                {
                    City = "   ",
                    Name = "Tech Park",
                    Street = "Antakalnio g.",
                    BuildingNumber = 12,
                    IsFree = false,
                }
            }
        };
    }

    public static WifiNetworkDto GetValidWifiNetworkDto(UserLoginResponseDto user)
    {
        return new WifiNetworkDto
        {
            UserId = user.Id!,
            City = "Kaunas",
            Name = "City Hotels Algirdas",
            Street = "Algirdo g.",
            BuildingNumber = 24,
            IsFree = true
        };
    }
}