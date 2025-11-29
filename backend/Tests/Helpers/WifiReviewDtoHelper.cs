using API.Domain.Dto;
using MongoDB.Bson;

namespace Tests.Helpers;

public static class WifiReviewDtoHelper
{
    public static IEnumerable<object[]> InvalidWifiReviewDtos()
    {
        return new List<object[]>
        {
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = "u1wfv4jn1w2uv982349",
                    Text = "    ",
                    Rating = 9,
                    City = "Vilnius",
                    Street = "Gedimino pr.",
                    BuildingNumber = 10
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = "user9876",
                    Text = "Very poor signal, disconnects often.",
                    Rating = 3,
                    City = "Kaunas",
                    Street = "Savanorių pr.",
                    BuildingNumber = 22
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Okayish speed, usable for browsing.",
                    Rating = 0,
                    City = "Vilnius",
                    Street = "Ozo g.",
                    BuildingNumber = 18
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = new string('a', 122),
                    Rating = 7,
                    City = "Klaipėda",
                    Street = "H. Manto g.",
                    BuildingNumber = 7
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Terrible experience!",
                    Rating = 11,
                    City = "Vilnius",
                    Street = "Pylimo g.",
                    BuildingNumber = 21
                }
            }
        };
    }
    
    public static IEnumerable<object[]> ValidWifiReviewDtos()
    {
        return new List<object[]>
        {
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Good connection, does not lag, NICE",
                    Rating = 9,
                    City = "Vilnius",
                    Street = "Jasinskio g.",
                    BuildingNumber = 56
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Very poor signal, disconnects often.",
                    Rating = 3,
                    City = "Kaunas",
                    Street = "Laisvės al.",
                    BuildingNumber = 45
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Okayish speed, usable for browsing.",
                    Rating = 6,
                    City = "Klaipėda",
                    Street = "Turgaus g.",
                    BuildingNumber = 12
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = new string("good enough"),
                    Rating = 7,
                    City = "Vilnius",
                    Street = "Ozo g.",
                    BuildingNumber = 18
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Terrible experience!",
                    Rating = 2,
                    City = "Kaunas",
                    Street = "Savanorių pr.",
                    BuildingNumber = 22
                }
            }
        };
    }
}