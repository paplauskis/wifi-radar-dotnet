using API.Data.Repositories.Interfaces;
using API.Domain;
using API.Domain.Dto;
using API.Services.Interfaces.Wifi;
using API.Domain.Models;
using API.Domain.Exceptions;
using MongoDB.Bson;

namespace API.Services.Wifi;

public class WifiReviewService : IWifiReviewService
{
    private readonly IWifiRepository _wifiRepository;
    private readonly IWifiReviewRepository _wifiReviewRepository;

    public WifiReviewService(IWifiRepository repo, IWifiReviewRepository wifiReviewRepository)
    {
        _wifiRepository = repo;
        _wifiReviewRepository = wifiReviewRepository;
    }

    public async Task<List<WifiReview>> GetReviewsAsync(string city, string street, int buildingNumber)
    {
        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(street) || buildingNumber < 1)
        {
            throw new ArgumentException($"City ({city}), street ({street}), or building number ({buildingNumber}) values are invalid.");
        }

        List<WifiReview> reviewsList = await _wifiReviewRepository.GetReviewsByAddressAsync(city, street, buildingNumber);
        
        if (reviewsList == null || reviewsList.Count == 0)
            throw new EmptyResponseException("");
        
        return reviewsList;
    }

    public async Task<WifiReview> AddReviewAsync(WifiReviewDto wifiReviewDto)
    {
        if (wifiReviewDto == null)
        {
            throw new ArgumentNullException(nameof(wifiReviewDto), "Passed in wifi object is null");
        }
        
        var newReview = new WifiReview
        {
            WifiId = ObjectId.GenerateNewId().ToString(),
            UserId = wifiReviewDto.UserId,
            Rating = (int)wifiReviewDto.Rating,
            Text = wifiReviewDto.Text,
            City = wifiReviewDto.City,
            Street = wifiReviewDto.Street,
            BuildingNumber = (int)wifiReviewDto.BuildingNumber,
            CreatedAt = DateTime.UtcNow
        };
        
        await _wifiReviewRepository.AddReviewAsync(newReview);
        return newReview;
    }
}
