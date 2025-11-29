using API.Data.Repositories;
using API.Data.Repositories.Interfaces;
using API.Domain.Models;
using API.Services.Auth;
using API.Services.Interfaces.Auth;
using API.Services.Interfaces.Map;
using API.Services.Interfaces.User;
using API.Services.Interfaces.Wifi;
using API.Services.Map;
using API.Services.Users;
using API.Services.Wifi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace Tests.ControllerTests;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private MongoDbRunner _runner;
    public IMongoDatabase TestDatabase { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var mongoClientDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IMongoClient));
            if (mongoClientDescriptor != null)
                services.Remove(mongoClientDescriptor);

            var mongoDatabaseDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IMongoDatabase));
            if (mongoDatabaseDescriptor != null)
                services.Remove(mongoDatabaseDescriptor);

            _runner = MongoDbRunner.Start(singleNodeReplSet: false);
            var client = new MongoClient(_runner.ConnectionString);
            var database = client.GetDatabase("JavaWebService");

            TestDatabase = database;

            services.AddSingleton<IMongoClient>(client);
            services.AddSingleton(database);
            
            services.RemoveAll<IUserRepository>();
            services.RemoveAll<IWifiRepository>();
            services.RemoveAll<IWifiReviewRepository>();
            services.RemoveAll<IWifiReviewService>();
            services.RemoveAll<IWifiPasswordSharingService>();
            services.RemoveAll<IUserAuthService>();
            services.RemoveAll<IUserFavoriteService>();
            services.RemoveAll<IUserReviewService>();
            services.RemoveAll<IMapService>();
            services.RemoveAll<IPasswordHasher<User>>();
            services.RemoveAll<IPasswordHelper>();
            services.RemoveAll<IAuthenticatable>();
            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWifiRepository, WifiRepository>();
            services.AddScoped<IWifiReviewRepository, WifiReviewRepository>();
            services.AddScoped<IWifiReviewService, WifiReviewService>();
            services.AddScoped<IUserFavoriteService, UserFavoriteService>();
            services.AddScoped<IUserAuthService, UserAuthService>();
            services.AddScoped<IWifiPasswordSharingService, WifiPasswordSharingService>();
            services.AddScoped<IMapService, MapService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IPasswordHelper, PasswordService>();
            services.AddScoped<IAuthenticatable, JwtService>();
        });
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _runner?.Dispose();
        return Task.CompletedTask;
    }
}