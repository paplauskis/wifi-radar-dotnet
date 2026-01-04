using System.IO;
using API.Domain.Dto;
using API.Domain.Exceptions;
using API.Exceptions;
using API.Helpers.Mappers;
using API.Services.Users;
using MongoDB.Bson;
using MongoDB.Driver;
using Tests.RepositoryTests;
using Xunit;
using UserModel = API.Domain.Models.User;
using WifiNetworkModel = API.Domain.Models.WifiNetwork;

namespace Tests.ServiceTests.User;

public class UserFavoriteTests
{
    private const string UsersCollection = "Users";
    private const string FavoritesCollection = "Favorite-WiFi-Spots";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetUserFavoritesAsync_WithEmptyUserId_ThrowsInvalidDataException(string? userId)
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var service = new UserFavoriteService(context.Database);

            await Assert.ThrowsAsync<InvalidDataException>(
                () => service.GetUserFavoritesAsync(userId!));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task GetUserFavoritesAsync_WithInvalidUserId_ThrowsInvalidDataException()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var service = new UserFavoriteService(context.Database);

            await Assert.ThrowsAsync<InvalidDataException>(
                () => service.GetUserFavoritesAsync("not-an-object-id"));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task GetUserFavoritesAsync_WithNoFavorites_ReturnsEmptyList()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var users = context.Database.GetCollection<UserModel>(UsersCollection);
            var userId = ObjectId.GenerateNewId().ToString();
            await users.InsertOneAsync(CreateUser(userId));

            var service = new UserFavoriteService(context.Database);
            var result = await service.GetUserFavoritesAsync(userId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task GetUserFavoritesAsync_WithMissingWifiNetworks_ThrowsNotFoundException()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var users = context.Database.GetCollection<UserModel>(UsersCollection);
            var userId = ObjectId.GenerateNewId().ToString();
            var wifiId = ObjectId.GenerateNewId().ToString();
            await users.InsertOneAsync(CreateUser(userId, new List<string> { wifiId }));

            var service = new UserFavoriteService(context.Database);

            await Assert.ThrowsAsync<NotFoundException>(
                () => service.GetUserFavoritesAsync(userId));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task GetUserFavoritesAsync_WithFavorites_ReturnsDtos()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var users = context.Database.GetCollection<UserModel>(UsersCollection);
            var wifiNetworks = context.Database.GetCollection<WifiNetworkModel>(FavoritesCollection);

            var userId = ObjectId.GenerateNewId().ToString();
            var wifiId = ObjectId.GenerateNewId().ToString();
            await users.InsertOneAsync(CreateUser(userId, new List<string> { wifiId }));

            var dto = CreateWifiDto(userId, wifiId);
            await wifiNetworks.InsertOneAsync(Mapper.MapDtoToWifiNetwork(dto));

            var service = new UserFavoriteService(context.Database);
            var result = await service.GetUserFavoritesAsync(userId);

            Assert.Single(result);
            Assert.Equal(dto.City, result[0].City);
            Assert.Equal(dto.Street, result[0].Street);
            Assert.Equal(dto.BuildingNumber, result[0].BuildingNumber);
            Assert.Equal(dto.UserId, result[0].UserId);
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("bad-id")]
    public async Task AddUserFavoriteAsync_WithInvalidUserId_ThrowsInvalidInputException(string? userId)
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var service = new UserFavoriteService(context.Database);
            var dto = CreateWifiDto(ObjectId.GenerateNewId().ToString());

            await Assert.ThrowsAsync<InvalidInputException>(
                () => service.AddUserFavoriteAsync(userId!, dto));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task AddUserFavoriteAsync_WithMissingUser_ThrowsUserNotFoundException()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var service = new UserFavoriteService(context.Database);
            var userId = ObjectId.GenerateNewId().ToString();
            var dto = CreateWifiDto(userId);

            await Assert.ThrowsAsync<UserNotFoundException>(
                () => service.AddUserFavoriteAsync(userId, dto));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task AddUserFavoriteAsync_WithDuplicateWifi_ThrowsWifiNetworkAlreadyExistsException()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var users = context.Database.GetCollection<UserModel>(UsersCollection);
            var wifiNetworks = context.Database.GetCollection<WifiNetworkModel>(FavoritesCollection);

            var userId = ObjectId.GenerateNewId().ToString();
            await users.InsertOneAsync(CreateUser(userId));

            var dto = CreateWifiDto(userId, ObjectId.GenerateNewId().ToString());
            await wifiNetworks.InsertOneAsync(Mapper.MapDtoToWifiNetwork(dto));

            var service = new UserFavoriteService(context.Database);

            await Assert.ThrowsAsync<WifiNetworkAlreadyExistsException>(
                () => service.AddUserFavoriteAsync(userId, dto));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task AddUserFavoriteAsync_WithNewWifi_AddsFavoriteAndWifi()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var users = context.Database.GetCollection<UserModel>(UsersCollection);
            var wifiNetworks = context.Database.GetCollection<WifiNetworkModel>(FavoritesCollection);

            var userId = ObjectId.GenerateNewId().ToString();
            await users.InsertOneAsync(CreateUser(userId));

            var dto = CreateWifiDto(userId);
            var service = new UserFavoriteService(context.Database);
            var result = await service.AddUserFavoriteAsync(userId, dto);

            Assert.False(string.IsNullOrWhiteSpace(result.WifiId));

            var updatedUser = await users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            Assert.NotNull(updatedUser);
            Assert.Contains(result.WifiId!, updatedUser!.FavoriteNetworkId);

            var insertedWifi = await wifiNetworks.Find(w => w.Id == result.WifiId).FirstOrDefaultAsync();
            Assert.NotNull(insertedWifi);
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task DeleteUserFavoriteAsync_WithEmptyUserId_ThrowsArgumentNullException()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var service = new UserFavoriteService(context.Database);

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => service.DeleteUserFavoriteAsync(null!, "City", "Street", 1));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Theory]
    [InlineData(null, "Street", 1)]
    [InlineData("City", null, 1)]
    [InlineData("", "Street", 1)]
    [InlineData("City", "", 1)]
    [InlineData("City", "Street", 0)]
    public async Task DeleteUserFavoriteAsync_WithInvalidLocation_ThrowsArgumentException(
        string? city,
        string? street,
        int buildingNumber)
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var service = new UserFavoriteService(context.Database);
            var userId = ObjectId.GenerateNewId().ToString();

            await Assert.ThrowsAsync<ArgumentException>(
                () => service.DeleteUserFavoriteAsync(userId, city!, street!, buildingNumber));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task DeleteUserFavoriteAsync_WithMissingUser_ThrowsUserNotFoundException()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var service = new UserFavoriteService(context.Database);
            var userId = ObjectId.GenerateNewId().ToString();

            await Assert.ThrowsAsync<UserNotFoundException>(
                () => service.DeleteUserFavoriteAsync(userId, "City", "Street", 1));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task DeleteUserFavoriteAsync_WithExistingWifi_DeletesWifiNetwork()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var users = context.Database.GetCollection<UserModel>(UsersCollection);
            var wifiNetworks = context.Database.GetCollection<WifiNetworkModel>(FavoritesCollection);

            var userId = ObjectId.GenerateNewId().ToString();
            await users.InsertOneAsync(CreateUser(userId));

            var dto = CreateWifiDto(userId, ObjectId.GenerateNewId().ToString());
            await wifiNetworks.InsertOneAsync(Mapper.MapDtoToWifiNetwork(dto));

            var service = new UserFavoriteService(context.Database);
            await service.DeleteUserFavoriteAsync(userId, dto.City!, dto.Street!, dto.BuildingNumber!.Value);

            var remaining = await wifiNetworks.Find(w => w.UserId == userId).ToListAsync();
            Assert.Empty(remaining);
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Theory]
    [InlineData(null, "wifi")]
    [InlineData("", "wifi")]
    [InlineData("user", null)]
    [InlineData("user", "")]
    public async Task IsFavoriteAsync_WithInvalidArgs_ThrowsInvalidDataException(string? username, string? wifiId)
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var service = new UserFavoriteService(context.Database);

            await Assert.ThrowsAsync<InvalidDataException>(
                () => service.IsFavoriteAsync(username!, wifiId!));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task IsFavoriteAsync_WithMissingUser_ThrowsUserNotFoundException()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var service = new UserFavoriteService(context.Database);

            await Assert.ThrowsAsync<UserNotFoundException>(
                () => service.IsFavoriteAsync("missing-user", ObjectId.GenerateNewId().ToString()));
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task IsFavoriteAsync_WhenFavoriteExists_ReturnsTrue()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var users = context.Database.GetCollection<UserModel>(UsersCollection);

            var wifiId = ObjectId.GenerateNewId().ToString();
            var userId = ObjectId.GenerateNewId().ToString();
            var username = $"user_{userId[..6]}";
            await users.InsertOneAsync(CreateUser(userId, new List<string> { wifiId }, username));

            var service = new UserFavoriteService(context.Database);
            var result = await service.IsFavoriteAsync(username, wifiId);

            Assert.True(result);
        }
        finally
        {
            context?.Dispose();
        }
    }

    [Fact]
    public async Task IsFavoriteAsync_WhenFavoriteMissing_ReturnsFalse()
    {
        TestDbContext? context = null;
        try
        {
            context = new TestDbContext(UsersCollection);
            var users = context.Database.GetCollection<UserModel>(UsersCollection);

            var userId = ObjectId.GenerateNewId().ToString();
            var username = $"user_{userId[..6]}";
            await users.InsertOneAsync(CreateUser(userId, new List<string>(), username));

            var service = new UserFavoriteService(context.Database);
            var result = await service.IsFavoriteAsync(username, ObjectId.GenerateNewId().ToString());

            Assert.False(result);
        }
        finally
        {
            context?.Dispose();
        }
    }

    private static UserModel CreateUser(string userId, List<string>? favoriteIds = null, string? username = null)
    {
        return new UserModel
        {
            Id = userId,
            Username = username ?? $"user_{userId[..6]}",
            Password = "Password1!",
            FavoriteNetworkId = favoriteIds ?? new List<string>()
        };
    }

    private static WifiNetworkDto CreateWifiDto(string userId, string? wifiId = null)
    {
        return new WifiNetworkDto
        {
            WifiId = wifiId,
            UserId = userId,
            City = "Vilnius",
            Name = "Cafe Spot",
            Street = "Main St",
            BuildingNumber = 1,
            IsFree = true
        };
    }
}
