using System.Text;
using API.Data.Repositories;
using API.Data.Repositories.Interfaces;
using API.Domain.Models;
using API.Services.Auth;
using API.Services.Database;
using API.Services.Interfaces.Auth;
using API.Services.Interfaces.Map;
using API.Services.Interfaces.User;
using API.Services.Interfaces.Wifi;
using API.Services.Map;
using API.Services.Users;
using API.Services.Wifi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

public class Program
{
    public static WebApplication CreateApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<MongoDbService>();
        builder.Services.AddSingleton<IMongoDatabase>(sp =>
        {
            var service = sp.GetRequiredService<MongoDbService>();
            return service.GetDatabase();
        });
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IWifiRepository, WifiRepository>();
        builder.Services.AddScoped<IWifiReviewRepository, WifiReviewRepository>();
        builder.Services.AddScoped<IWifiReviewService, WifiReviewService>();
        builder.Services.AddScoped<IUserFavoriteService, UserFavoriteService>();
        builder.Services.AddScoped<IUserAuthService, UserAuthService>();
        builder.Services.AddScoped<IWifiPasswordSharingService, WifiPasswordSharingService>();
        builder.Services.AddScoped<IUserReviewService, UserReviewService>();
        builder.Services.AddScoped<IMapService, MapService>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IPasswordHelper, PasswordService>();
        builder.Services.AddScoped<IAuthenticatable, JwtService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; //should be true if pushed to production
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["JwtConfig:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
            };
        });

        var app = builder.Build();

        app.UseCors("AllowFrontend");
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        if (app.Environment.IsDevelopment())
        {
             app.UseSwagger();
             app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseAuthentication();

        app.MapControllers();

        return app;
    }

    public static void Main(string[] args)
    {
        CreateApp(args).Run();
    }
}