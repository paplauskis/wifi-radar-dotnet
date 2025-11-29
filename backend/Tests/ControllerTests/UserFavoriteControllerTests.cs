using System.Net;
using System.Net.Http.Json;
using System.Text;
using API.Domain.Dto;
using MongoDB.Bson;
using Newtonsoft.Json;
using Tests.Helpers;
using Xunit;

namespace Tests.ControllerTests;

public class UserFavoriteControllerTests
{
    private const string ApiUri = "/api/user";
    
    [Fact]
    public async Task AddFavorite_WithValidData_ShouldReturnCreated()
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        var user = await CreateSampleUser(client);
        var wifiNetwork = WifiNetworkDtoHelper.GetValidWifiNetworkDto(user);
        
        var addFavoriteContent = new StringContent(JsonConvert.SerializeObject(wifiNetwork), Encoding.UTF8, "application/json");
        var addFavoriteResponse = await client.PostAsync($"{ApiUri}/{user.Id}/favorites", addFavoriteContent);
        var addFavoriteResult = await addFavoriteResponse.Content.ReadAsStringAsync();
    
        Assert.Equal(HttpStatusCode.OK, addFavoriteResponse.StatusCode);
        Assert.Contains(wifiNetwork.WifiId, addFavoriteResult);
        Assert.Contains(wifiNetwork.UserId, addFavoriteResult);
        Assert.Contains(wifiNetwork.City, addFavoriteResult);
        Assert.Contains(wifiNetwork.Name, addFavoriteResult);
        Assert.Contains(wifiNetwork.Street, addFavoriteResult);
        Assert.Contains(wifiNetwork.BuildingNumber.ToString()!, addFavoriteResult);
    }

    [Theory]
    [MemberData(nameof(WifiNetworkDtoHelper.InvalidWifiNetworkDtos), MemberType = typeof(WifiNetworkDtoHelper))]
    public async Task AddFavorite_WithInvalidDtoData_ShouldReturnBadRequest(WifiNetworkDto wifiNetwork)
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        var user = await CreateSampleUser(client);
        wifiNetwork.UserId = user.Id!;
        
        var addFavoriteContent = new StringContent(JsonConvert.SerializeObject(wifiNetwork), Encoding.UTF8, "application/json");
        var addFavoriteResponse = await client.PostAsync($"{ApiUri}/{user.Id}/favorites", addFavoriteContent);
        var addFavoriteResult = await addFavoriteResponse.Content.ReadAsStringAsync();
        
        Assert.Equal(HttpStatusCode.BadRequest, addFavoriteResponse.StatusCode);
        Assert.Contains($"WifiNetworkDto parameter data is not valid", addFavoriteResult);
    }

    [Theory]
    [InlineData("invalidUserId")]
    [InlineData("71cthn8049")]
    [InlineData("id")]
    public async Task AddFavorite_WithInvalidUserId_ShouldReturnNotFound(string? invalidUserId)
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        var user = await CreateSampleUser(client);
        var wifiNetwork = WifiNetworkDtoHelper.GetValidWifiNetworkDto(user);
        
        var addFavoriteContent = new StringContent(JsonConvert.SerializeObject(wifiNetwork), Encoding.UTF8, "application/json");
        var addFavoriteResponse = await client.PostAsync($"{ApiUri}/{invalidUserId}/favorites", addFavoriteContent);
        var addFavoriteResult = await addFavoriteResponse.Content.ReadAsStringAsync();
        
        Assert.Equal(HttpStatusCode.NotFound, addFavoriteResponse.StatusCode);
        Assert.Equal($"User ID \"{invalidUserId}\" is not valid", addFavoriteResult);
    }
    
    [Fact]
    public async Task AddFavorite_WithDuplicateWifiNetwork_ShouldReturnConflict()
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        var user = await CreateSampleUser(client);
        var wifiNetwork = WifiNetworkDtoHelper.GetValidWifiNetworkDto(user);
        
        var addFavoriteContent = new StringContent(JsonConvert.SerializeObject(wifiNetwork), Encoding.UTF8, "application/json");
        await client.PostAsync($"{ApiUri}/{user.Id}/favorites", addFavoriteContent);
        var addFavoriteResponse = await client.PostAsync($"{ApiUri}/{user.Id}/favorites", addFavoriteContent);
        var addFavoriteResult = await addFavoriteResponse.Content.ReadAsStringAsync();
    
        Assert.Equal(HttpStatusCode.Conflict, addFavoriteResponse.StatusCode);
        Assert.Equal("Wifi network is already saved to favorites by this user.", addFavoriteResult);
    }
    
    [Theory]
    [InlineData("invalidUserId")]
    [InlineData("71cthn8049")]
    [InlineData("id")]
    public async Task GetFavorites_WithInvalidUserId_ShouldReturnNotFound(string? invalidUserId)
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        var addFavoriteResponse = await client.GetAsync($"{ApiUri}/{invalidUserId}/favorites");
        var addFavoriteResult = await addFavoriteResponse.Content.ReadAsStringAsync();
        
        Assert.Equal(HttpStatusCode.NotFound, addFavoriteResponse.StatusCode);
        Assert.Equal($"User ID \"{invalidUserId}\" is not valid", addFavoriteResult);
    }

    [Fact]
    public async Task GetFavorites_WithValidUserIdAndNoFavorites_ShouldReturnNoContent()
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var user = await CreateSampleUser(client);
        
        var addFavoriteResponse = await client.GetAsync($"{ApiUri}/{user.Id}/favorites");
        
        Assert.Equal(HttpStatusCode.NoContent, addFavoriteResponse.StatusCode);
    }

    [Fact]
    public async Task GetFavorites_WithValidUserIdAndExistingFavorites_ShouldReturnOk()
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var user = await CreateSampleUser(client);
        var wifiNetwork = WifiNetworkDtoHelper.GetValidWifiNetworkDto(user);
        
        var addFavoriteContent = new StringContent(JsonConvert.SerializeObject(wifiNetwork), Encoding.UTF8, "application/json");
        var addFavoriteResponse = await client.PostAsync($"{ApiUri}/{user.Id}/favorites", addFavoriteContent);
        
        var getFavoritesResponse = await client.GetAsync($"{ApiUri}/{user.Id}/favorites");
        var getFavoritesResult = await getFavoritesResponse.Content.ReadFromJsonAsync<List<WifiNetworkDto>>();
        
        Assert.Equal(HttpStatusCode.OK, addFavoriteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getFavoritesResponse.StatusCode);
        Assert.NotNull(getFavoritesResult);
        Assert.Single(getFavoritesResult);
    }
    
    [Theory]
    [InlineData("invalidUserId", "684140ad072cafedbe6c6574")]
    [InlineData("71cthn8049", "684140ad072cafedbe6c6574")]
    [InlineData("id", "684140ad072cafedbe6c6574")]
    [InlineData("684140ad072cafedbe6c6574", "invalidWifiId")]
    [InlineData("684140ad072cafedbe6c6574", "tv134ynm8vmtu8")]
    [InlineData("684140ad072cafedbe6c6574", "id")]
    public async Task DeleteFavorite_WithInvalidUserOrWifiId_ShouldReturnNotFound(string? invalidUserId, string? invalidWifiId)
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        var addFavoriteResponse = await client.DeleteAsync($"{ApiUri}/{invalidUserId}/favorites/{invalidWifiId}");
        var addFavoriteResult = await addFavoriteResponse.Content.ReadAsStringAsync();
        
        Assert.Equal(HttpStatusCode.NotFound, addFavoriteResponse.StatusCode);
        Assert.Equal($"User ID or wifi ID is not valid", addFavoriteResult);
    }

    [Theory]
    [InlineData("684140ad072cafedbe1c6577", "684140ad072cafedbe6c6574")]
    [InlineData("684140ad072cafedbd6a6552", "684140ad072cafedbe6c1234")]
    public async Task DeleteFavorite_WithValidUserOrWifiId_ShouldReturnOk(string userId, string wifiId)
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        var response = await client.DeleteAsync($"{ApiUri}/{userId}/favorites/{wifiId}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFavorite_ShouldDeleteWifiNetwork_ReturnOk()
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var user = await CreateSampleUser(client);
        var wifiNetwork = WifiNetworkDtoHelper.GetValidWifiNetworkDto(user);
        
        // save wifi network to favorites
        var addFavoriteContent = new StringContent(JsonConvert.SerializeObject(wifiNetwork), Encoding.UTF8, "application/json");
        var addFavoriteResponse = await client.PostAsync($"{ApiUri}/{user.Id}/favorites", addFavoriteContent);
        
        // retrieve favorite wifi networks (should be 1)
        var getFavoritesResponse = await client.GetAsync($"{ApiUri}/{user.Id}/favorites");
        var getFavoritesResult = await getFavoritesResponse.Content.ReadFromJsonAsync<List<WifiNetworkDto>>();
        
        // delete the wifi network that was saved to favorites
        var deleteFavoriteResponse = await client.DeleteAsync($"{ApiUri}/{user.Id}/favorites/{wifiNetwork.WifiId}");
        
        // retrieve favorite wifi networks (should be 0)
        var getFavoritesResponse2 = await client.GetAsync($"{ApiUri}/{user.Id}/favorites");
        var getFavoritesResult2 = await getFavoritesResponse2.Content.ReadFromJsonAsync<List<WifiNetworkDto>>();

        Assert.Equal(HttpStatusCode.OK, addFavoriteResponse.StatusCode);
        Assert.NotNull(getFavoritesResult);
        Assert.Single(getFavoritesResult);
        Assert.Equal(HttpStatusCode.OK, deleteFavoriteResponse.StatusCode);
        Assert.NotNull(getFavoritesResult2);
        Assert.Empty(getFavoritesResult2);
    }
    
    private async Task<UserLoginResponseDto> CreateSampleUser(HttpClient client)
    {
        var user = new UserLoginRequestDto { Username = "sampleUser", Password = "randomPassword123" };
        var json = JsonConvert.SerializeObject(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{ApiUri}/auth/register", content);
        var userResponse = await response.Content.ReadFromJsonAsync<UserLoginResponseDto>();
        
        return userResponse;
    }
}