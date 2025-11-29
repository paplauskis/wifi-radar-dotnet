using System.Net;
using System.Net.Http.Json;
using System.Text;
using API.Domain;
using API.Domain.Dto;
using API.Domain.Models;
using Newtonsoft.Json;
using Tests.Helpers;
using Xunit;

namespace Tests.ControllerTests;

public class WifiControllerTests
{
    private const string ApiUri = "/api/wifi";
    
    [Theory]
    [MemberData(nameof(WifiReviewDtoHelper.InvalidWifiReviewDtos), MemberType = typeof(WifiReviewDtoHelper))]
    public async Task AddWifiReview_WithInvalidData_ShouldReturnBadRequest(WifiReviewDto wifiReviewDto)
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        var addReviewContent = new StringContent(JsonConvert.SerializeObject(wifiReviewDto), Encoding.UTF8, "application/json");
        var addWifiReviewResponse = await client.PostAsync($"{ApiUri}/reviews", addReviewContent);
        var addWifiReviewResult = await addWifiReviewResponse.Content.ReadAsStringAsync();
    
        Assert.Equal(HttpStatusCode.BadRequest, addWifiReviewResponse.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(addWifiReviewResult));
    }
    
    [Theory]
    [MemberData(nameof(WifiReviewDtoHelper.ValidWifiReviewDtos), MemberType = typeof(WifiReviewDtoHelper))]
    public async Task AddWifiReview_WithValidData_ShouldReturnOk(WifiReviewDto wifiReviewDto)
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var user = await CreateSampleUser(client);
        wifiReviewDto.UserId = user.Id ?? throw new Exception("User id is null");
        
        var addReviewContent = new StringContent(JsonConvert.SerializeObject(wifiReviewDto), Encoding.UTF8, "application/json");
        var addWifiReviewResponse = await client.PostAsync($"{ApiUri}/reviews", addReviewContent);
        
        Assert.Equal(HttpStatusCode.OK, addWifiReviewResponse.StatusCode);
        
        WifiReview? addWifiReviewResult = await addWifiReviewResponse.Content.ReadFromJsonAsync<WifiReview>();
        
        Assert.NotNull(addWifiReviewResult);
        Assert.Equal(wifiReviewDto.UserId, addWifiReviewResult.UserId);
        Assert.Equal(wifiReviewDto.Text, addWifiReviewResult.Text);
        Assert.Equal(wifiReviewDto.Rating, addWifiReviewResult.Rating);
    }

    [Theory]
    [InlineData("randomID12313")]
    [InlineData("i")]
    [InlineData("6839")]
    public async Task GetWifiReviews_WithInvalidWifiId_ShouldReturnBadRequest(string wifiId)
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        
        var getWifiReviewResponse = await client.GetAsync($"{ApiUri}/reviews");
        var getWifiReviewResult = await getWifiReviewResponse.Content.ReadAsStringAsync();
        
        Assert.Equal(HttpStatusCode.BadRequest, getWifiReviewResponse.StatusCode);
        Assert.Equal($"Invalid wifi id: \"{wifiId}\"", getWifiReviewResult);
    }

    //ensure AddWifiReview works first
    [Fact]
    public async Task GetWifiReviews_WhenReviewsExist_ShouldReturnOk()
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var user = await CreateSampleUser(client);
        var sampleWifiReview = (WifiReviewDto)WifiReviewDtoHelper.ValidWifiReviewDtos().ToList()[0][0];
        sampleWifiReview.UserId = user.Id ?? throw new Exception("User id is null");
        
        //add one wifi review
        var addReviewContent = new StringContent(JsonConvert.SerializeObject(sampleWifiReview), Encoding.UTF8, "application/json");
        var addWifiReviewResponse = await client.PostAsync($"{ApiUri}/reviews", addReviewContent);
        var addWifiReviewResult = await addWifiReviewResponse.Content.ReadFromJsonAsync<WifiReview>();
        
        //get wifi review (should be 1)
        var getWifiReviewResponse = await client.GetAsync($"{ApiUri}/reviews");
        var getWifiReviewResult = await getWifiReviewResponse.Content.ReadFromJsonAsync<List<WifiReview>>();
        
        Assert.Equal(HttpStatusCode.OK, getWifiReviewResponse.StatusCode);
        Assert.NotNull(getWifiReviewResult);
        Assert.Single(getWifiReviewResult);
        Assert.NotNull(addWifiReviewResult);
        Assert.NotNull(addWifiReviewResult.UserId);
        Assert.Equal(sampleWifiReview.UserId, addWifiReviewResult.UserId);
        Assert.Equal(sampleWifiReview.Text, addWifiReviewResult.Text);
        Assert.Equal(sampleWifiReview.Rating, addWifiReviewResult.Rating);
    }

    [Fact]
    public async Task GetWifiReviews_WhenReviewsDontExist_ShouldReturnNoContent()
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        await CreateSampleUser(client);
        var sampleWifiReview = (WifiReviewDto)WifiReviewDtoHelper.ValidWifiReviewDtos().ToList()[0][0];
        var wifiId = sampleWifiReview.WifiId ?? throw new Exception("Wifi id is null");
        
        var getWifiReviewResponse = await client.GetAsync($"{ApiUri}/reviews");
        
        Assert.Equal(HttpStatusCode.NoContent, getWifiReviewResponse.StatusCode);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("vnthuq0vqthun09v0qtuhnrv0qtunrhm9vmqtrvmrtghu9vthunm90rwtvwhnm89247t2vn0hmnw6urnw6unw6u2wu4bni5m213muf")]
    public async Task AddPassword_WithInvalidPassword_ShouldReturnBadRequest(string? password)
    {
        await using var factory = new ApiWebApplicationFactory();
        var client = factory.CreateClient();
        var passwordDto = new PasswordDto
        {
            City = "Klaipėda",
            Street = "Debreceno g.",
            BuildingNumber = 50,
            Password = password
        };
        
        var addPasswordContent = new StringContent(JsonConvert.SerializeObject(passwordDto), Encoding.UTF8, "application/json");
        var addPasswordResponse = await client.PostAsync($"{ApiUri}/passwords", addPasswordContent);
        var addPasswordResult = await addPasswordResponse.Content.ReadAsStringAsync();
        
        Assert.Equal(HttpStatusCode.BadRequest, addPasswordResponse.StatusCode);
        Assert.Contains($"Invalid password: \"{passwordDto.Password}\"", addPasswordResult);
    }
    
    private async Task<UserLoginResponseDto> CreateSampleUser(HttpClient client)
    {
        var user = new UserLoginRequestDto { Username = "sampleUser", Password = "randomPassword123" };
        var json = JsonConvert.SerializeObject(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"api/user/auth/register", content);
        var userResponse = await response.Content.ReadFromJsonAsync<UserLoginResponseDto>();
        
        return userResponse;
    }
}