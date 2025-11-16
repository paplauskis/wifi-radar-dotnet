using API.Domain.Dto;
using API.Domain.Models;

namespace API.Services.Interfaces.Wifi;

public interface IWifiReviewService
{
    Task<List<WifiReview>> GetReviewsAsync(string city, string street, int buildingNumber);
    Task<WifiReview> AddReviewAsync(WifiReviewDto wifiReviewDto);
}