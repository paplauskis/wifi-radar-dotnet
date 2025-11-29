using API.Domain.Dto;
using API.Domain.Exceptions;
using API.Domain.Models;
using API.Helpers.Mappers;
using API.Services.Interfaces.User;
using API.Exceptions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace API.Services.Users
{
    public class UserFavoriteService : IUserFavoriteService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<WifiNetwork> _wifiNetworks;

        public UserFavoriteService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
            _wifiNetworks = database.GetCollection<WifiNetwork>("Favorite-WiFi-Spots");
        }

        public async Task<List<WifiNetworkDto>> GetUserFavoritesAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidDataException("username cannot be null or empty.");
            
            if (!ObjectId.TryParse(userId, out _)) 
                throw new InvalidDataException($"User ID \"{userId}\" is not valid");
            
            var ids = await GetFavoriteNetworkIdsAsync(userId);
            if (ids == null || !ids.Any()) return new List<WifiNetworkDto>();

            var filter = Builders<WifiNetwork>.Filter.In(w => w.Id, ids);
            var wifiNetworkList = await _wifiNetworks.Find(filter).ToListAsync();
            
            if (wifiNetworkList == null || !wifiNetworkList.Any())
                throw new NotFoundException("None of the user's favorite Wifi networks were found.");

            var dtoList = new List<WifiNetworkDto>();
            foreach (var wifiNetwork in wifiNetworkList)
            {
                dtoList.Add(Mapper.MapWifiNetworkToDto(wifiNetwork));
            }

            return dtoList;
        }

        public async Task<WifiNetworkDto> AddUserFavoriteAsync(string userId, WifiNetworkDto wifi)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidInputException("Username cannot be null or empty.");
            
            if (!ObjectId.TryParse(userId, out _)) 
                throw new InvalidInputException($"User ID \"{userId}\" is not valid");
            
            if (wifi == null || string.IsNullOrWhiteSpace(wifi.WifiId))
                wifi.WifiId = ObjectId.GenerateNewId().ToString();

            var existingWifi = await _wifiNetworks.Find(w =>
            w.UserId == userId &&
            w.City == wifi.City &&
            w.Street == wifi.Street &&
            w.BuildingNumber == wifi.BuildingNumber
            ).FirstOrDefaultAsync();

            bool doesWifiAlreadyExist = existingWifi != null;
            if (doesWifiAlreadyExist)
                throw new WifiNetworkAlreadyExistsException(
                    "Wifi network is already saved to favorites by this user.", wifi);

            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new UserNotFoundException($"user with id {userId} could not be found.");
            
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.AddToSet(u => u.FavoriteNetworkId, wifi.WifiId);
            await _users.UpdateOneAsync(filter, update);

            var existing = await _wifiNetworks.Find(w => w.Id == wifi.WifiId).FirstOrDefaultAsync();
            if (existing == null)
            {
                await _wifiNetworks.InsertOneAsync(Mapper.MapDtoToWifiNetwork(wifi));
            }

            return wifi;
        }

        public async Task DeleteUserFavoriteAsync(string userId, string wifiId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidDataException("Username cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(wifiId))
                throw new InvalidDataException("WifiId cannot be null or empty");

            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new UserNotFoundException(userId);

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Pull(u => u.FavoriteNetworkId, wifiId);
            await _users.UpdateOneAsync(filter, update);
        }
        
        private async Task<List<string>> GetFavoriteNetworkIdsAsync(string userId)
        {
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new UserNotFoundException(userId);

            return user?.FavoriteNetworkId ?? new List<string>();
        }

        public async Task<bool> IsFavoriteAsync(string username, string wifiId)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidDataException("Username cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(wifiId))
                throw new InvalidDataException("WifiId cannot be null or empty.");

            var user = await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            if (user == null)
                throw new UserNotFoundException(username);

            return user?.FavoriteNetworkId?.Contains(wifiId) ?? false;
        }
    }
}