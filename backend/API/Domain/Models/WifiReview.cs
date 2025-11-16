using System.Text.Json.Serialization;
using DnsClient;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Domain.Models;

public class WifiReview : TimeStampedEntity
{
    private string _wifiId;

    [BsonElement("WifiNetworkID")]
    [JsonPropertyName("WifiNetworkID")]
    public string WifiId
    {
        get => _wifiId;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"WifiId cannot be set to {value}");
            } 
            _wifiId = value;
        }
    }

    private string _userId;
    [BsonElement("UserID")]
    [JsonPropertyName("UserID")]
    public string UserId
    {
        get =>_userId;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"UserId cannot be set to {value}");
            }
            _userId = value;
        }
    }

    private string? _text;

    [BsonElement("Comment")]
    [JsonPropertyName("Comment")]
    public string? Text
    {
        get => _text;
        set
        {
            if (value != null && (value.Length < 5 || value.Length > 100))
            {
                throw new ArgumentException("Text must be between 5 and 100 characters long");
            }
            _text = value;
        }
    }

    private int _rating;

    [BsonElement("Rating")]
    public int Rating
    {
        get => _rating;
        set
        {
            if (value < 1 || value > 10)
                throw new ArgumentException("Rating must be between 1 and 10");
            _rating = value;
        }
    }
    
    private string _city;

    [BsonElement("City")]
    public string City
    {
        get => _city;
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
            
            if (value.Length > 100)
                throw new ArgumentException("City name length must be less than 100 characters");
            
            _city = value;
        }
    }
    
    private string _street;

    [BsonElement("Street")]
    public string Street
    {
        get => _street;
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
            
            if (value.Length > 100)
                throw new ArgumentException("Street name length must be less than 100 characters");
            
            _street = value;
        }
    }
    
    private int _buildingNumber;

    [BsonElement("BuildingNumber")]
    public int BuildingNumber
    {
        get => _buildingNumber;
        set
        {
            if (value < 1 || value > 10000)
                throw new ArgumentException("Invalid building number: " + value);
            
            _buildingNumber = value;
        }
    }
}