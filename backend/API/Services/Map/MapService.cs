using System.Text;
using System.Text.Json;
using API.Domain.Dto.IpapiDto;
using API.Domain.Dto.OverpassDto;
using API.Domain.Exceptions;
using API.Helpers;
using API.Services.Interfaces.Map;

namespace API.Services.Map;

public class MapService : IMapService
{
    private readonly HttpClient _client = new();
    
    public async Task<List<OverpassResponseElementDto>> Search(string city, int? radius)
    {
        if (string.IsNullOrEmpty(city)) 
            throw new ArgumentNullException(nameof(city), "City cannot be null or empty");

        List<OverpassResponseElementDto> wifis;
        
        if (radius == null)
        {
            wifis = await SearchInCity(city);
        }
        else
        {
            if (radius < 0) 
                throw new ArgumentException($"Radius cannot be negative; selected radius is {radius} meters");
            if (radius > 100000)
                throw new ArgumentException($"Radius cannot be greater than 100 kilometers; selected radius is {radius / 100} kilometers"); 
            
            wifis = await SearchInRadius(city, (int)radius);
        }

        if (wifis.Count == 0)
            throw new EmptyResponseException("No wifi networks were found in the selected location.");
        
        return wifis;
    }

    public async Task<List<OverpassResponseElementDto>> SearchWifisFromNearestCity(string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentNullException(nameof(ip), "IP is null or empty");

        var city = await GetCityFromIp(ip) 
                   ?? throw new ArgumentNullException(nameof(ip), "Cannot get city from IP");
        
        var wifis = await SearchInCity(city);
        
        if (wifis.Count == 0)
            throw new EmptyResponseException("No wifi networks were found in city");

        return wifis;
    }

    public async Task<CoordinateDto> GetCoordinates(string city, string street, int buildingNumber)
    {
        if (
            string.IsNullOrEmpty(city) || 
            string.IsNullOrEmpty(street) || 
            buildingNumber < 1)
        {
            throw new ArgumentException("City, street or building number value is null or empty");
        }
        
        var content = new StringContent(
            OverpassApi.WifiCoordinates(city, street, buildingNumber), 
            Encoding.UTF8, 
            "application/json");

        var responseDto = await PostResponseOverpassApi(content);

        if (responseDto == null)
            throw new ArgumentNullException(nameof(responseDto));
        
        if (responseDto.Elements.Count == 0)
            throw new EmptyResponseException("No network was found with this address");
        
        if (responseDto.Elements.Count > 1)
            throw new ArgumentException("Multiple networks were found with this address, should be 1");
        
        
        return new CoordinateDto
        {
            Latitude = responseDto.Elements[0].Latitude.ToString(), 
            Longitude = responseDto.Elements[0].Longitude.ToString()
        };
    }

    private async Task<List<OverpassResponseElementDto>> SearchInCity(string city)
    {
        List<OverpassResponseElementDto> wifis = new();
        var content = new StringContent(
            OverpassApi.FreeWifiInCity(city), 
            Encoding.UTF8, 
            "application/json");

        var deserializedObj = await PostResponseOverpassApi(content);

        if (deserializedObj != null && deserializedObj.Elements != null)
        {
            foreach (var elem in deserializedObj.Elements)
            {
                wifis.Add(elem);
            }
        }
        
        return wifis;
    }

    private async Task<List<OverpassResponseElementDto>> SearchInRadius(string city, int radius)
    {
        var coordinateDto = await GetCityCenterPoint(city);
        
        if (coordinateDto == null)
            throw new ArgumentNullException(nameof(coordinateDto), "City center point cannot be null; check city name and try again.");
        
        List<OverpassResponseElementDto> wifis = new();
        var content = new StringContent(
            OverpassApi.FreeWifiInRadius(radius, coordinateDto.Latitude, coordinateDto.Longitude), 
            Encoding.UTF8, 
            "application/json");
        
        var deserializedObj = await PostResponseOverpassApi(content);

        if (deserializedObj != null && deserializedObj.Elements != null)
        {
            foreach (var elem in deserializedObj.Elements)
            {
                wifis.Add(elem);
            }
        }
        
        return wifis;
    }

    private async Task<OverpassResponseDto?> PostResponseOverpassApi(StringContent content)
    {
        var response = await _client.PostAsync(OverpassApi.ApiUrl, content);
        var responseString = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<OverpassResponseDto>(responseString);
    }

    private async Task<CoordinateDto?> GetCityCenterPoint(string city)
    {
        _client.DefaultRequestHeaders.UserAgent.ParseAdd("user_agent"); //user agent is needed to access openstreetmap api
        var response = await _client.GetAsync(OpenstreetmapApi.CityCenterPoint(city));
        var responseString = await response.Content.ReadAsStringAsync();
        
        var dto = JsonSerializer.Deserialize<List<CoordinateDto>>(responseString);

        if (dto == null || dto.Count == 0)
        {
            return null;
        }
        
        return dto.First();
    }

    private async Task<string?> GetCityFromIp(string ip)
    {
        var apiUrl = IpapiApi.GetRequestUrlProperty(ip, "city");
        var response = await _client.GetAsync(apiUrl);
        return await response.Content.ReadAsStringAsync();
    }
}