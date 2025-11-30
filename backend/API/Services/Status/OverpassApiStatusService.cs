using System.Diagnostics;
using System.Text;
using API.Domain.Dto;
using API.Helpers;
using API.Services.Interfaces.Status;

namespace API.Services.Status;

public class OverpassApiStatusService : IOverpassApiStatusService
{
    private readonly string[] _cities = ["Vilnius", "Kaunas", "Klaipėda", "Šiauliai", "Alytus", "Panevėžys"];
    private readonly HttpClient _client = new();
    public async Task<ApiStatusDto> CheckStatus()
    {
        var tasks = _cities.Select(GetRequestData);
        var results = await Task.WhenAll(tasks);
        var isStable = results.All(x => (int)x.StatusCode >= 200 && (int)x.StatusCode <= 299);
        
        return new ApiStatusDto
        {
            IsStable = isStable,
            Responses = results.ToList()
        };
    }

    private async Task<ApiResponseDto> GetRequestData(string city)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var response = await ExecuteRequest(city);
        stopwatch.Stop();

        return new ApiResponseDto
        {
            StatusCode = response.StatusCode,
            Message = response.Content.ReadAsStringAsync().Result,
            ResponseTimeMs = stopwatch.ElapsedMilliseconds
        };
    }

    private async Task<HttpResponseMessage> ExecuteRequest(string city)
    {
        var bodyMessage = OverpassApi.FreeWifiInCity(city).Replace("out body;", "out 1;"); // make response return only 1 result, it is enough
        var content = new StringContent(bodyMessage, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(OverpassApi.ApiUrl, content);
        
        return response;
    }
}