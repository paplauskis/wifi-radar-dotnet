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
                    WifiId = "randomID12313",
                    UserId = "u1wfv4jn1w2uv982349",
                    Text = "Good connection, does not lag, NICE",
                    Rating = 9
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    WifiId = "i",
                    UserId = "user9876",
                    Text = "Very poor signal, disconnects often.",
                    Rating = 3
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    WifiId = ObjectId.GenerateNewId().ToString(),
                    UserId = "rthb",
                    Text = "Okayish speed, usable for browsing.",
                    Rating = 6
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    WifiId = ObjectId.GenerateNewId().ToString(),
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = new string('a', 122),
                    Rating = 7
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    WifiId = ObjectId.GenerateNewId().ToString(),
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Terrible experience!",
                    Rating = 11
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
                    WifiId = ObjectId.GenerateNewId().ToString(),
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Good connection, does not lag, NICE",
                    Rating = 9
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    WifiId = ObjectId.GenerateNewId().ToString(),
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Very poor signal, disconnects often.",
                    Rating = 3
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    WifiId = ObjectId.GenerateNewId().ToString(),
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Okayish speed, usable for browsing.",
                    Rating = 6
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    WifiId = ObjectId.GenerateNewId().ToString(),
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = new string("good enough"),
                    Rating = 7
                }
            },
            new object[]
            {
                new WifiReviewDto
                {
                    WifiId = ObjectId.GenerateNewId().ToString(),
                    UserId = ObjectId.GenerateNewId().ToString(),
                    Text = "Terrible experience!",
                    Rating = 2
                }
            }
        };
    }
}