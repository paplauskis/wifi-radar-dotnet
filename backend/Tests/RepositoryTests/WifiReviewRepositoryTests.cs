using API.Data.Repositories;
using API.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Xunit;

namespace Tests.RepositoryTests;

public class WifiReviewRepositoryTests: BaseRepositoryTests<WifiReview, WifiReviewRepository>
{
    private const string CollectionName = "Reviews";
    
    public WifiReviewRepositoryTests() : base(CollectionName) {}

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public override async Task AddAsync_ShouldInsertEntityIntoCollection(int num)
    {
        try
        {
            Context = new TestDbContext(CollectionName);
            Repo = GetRepository(Context);
            Collection = Context.Database.GetCollection<WifiReview>(CollectionName);

            var expected = await Repo.AddAsync(TestData[num]);

            var actual = await Collection
                .Find(e => e.Id == expected.Id)
                .FirstOrDefaultAsync();

            AssertWifiReviewValuesEqual(expected, actual);
        }
        finally
        {
            Context?.Dispose(); 
        }
    }

    [Theory]
    [MemberData(nameof(ValidObjects))]
    public override async Task GetByIdAsync_ShouldReturnCorrectEntity_WhenIdExists(WifiReview entity)
    {
        try
        {
            Context = new TestDbContext(CollectionName);
            Repo = GetRepository(Context);
            Collection = Context.Database.GetCollection<WifiReview>(CollectionName);
            await Collection.InsertManyAsync(TestData);

            var expected = entity;
            var actual = await Repo.GetByIdAsync(entity.Id);

            Assert.NotNull(actual);
            AssertWifiReviewValuesEqual(expected, actual);
        }
        finally
        {
            Context?.Dispose(); 
        }
    }

    [Theory]
    [MemberData(nameof(ValidObjects))]
    public async Task GetReviewsByWifiIdAsync_ShouldReturnCorrectReviews_WhenWifiIdExists(WifiReview review)
    {
        try
        {
            Context = new TestDbContext(CollectionName);
            Repo = GetRepository(Context);
            Collection = Context.Database.GetCollection<WifiReview>(CollectionName);
            await Collection.InsertManyAsync(TestData);
    
            var expected = TestData
                .Where(e => e.City == review.City 
                            && e.Street == review.Street 
                            && e.BuildingNumber == review.BuildingNumber)
                .OrderBy(e => e.CreatedAt)
                .ToList();
            var fetched = await Repo.GetReviewsByAddressAsync(review.City, review.Street, review.BuildingNumber);
            var actual = fetched
                .OrderBy(e => e.CreatedAt)
                .ToList();
    
    
            Assert.NotNull(actual);
            Assert.Equal(expected.Count, actual.Count);
    
            for (int i = 0; i < expected.Count; i++)
            {
                AssertWifiReviewValuesEqual(expected[i], actual[i]);
            }
        }
        finally
        {
            Context?.Dispose(); 
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("RANDOM_ID")]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("RANDOM_ID", true)]
    public async Task GetReviewsByWifiIdAsync_ShouldReturnNoReviews_WhenWifiIdIsInvalid(string? wifiId, bool emptyCollection = false)
    {
        try
        {
            Context = new TestDbContext(CollectionName);
            Repo = GetRepository(Context);
            Collection = Context.Database.GetCollection<WifiReview>(CollectionName);

            if (!emptyCollection)
            {
                await Collection.InsertManyAsync(TestData);
            }
            
            var fetched = await Repo.GetReviewsByWifiIdAsync(wifiId);
            var actual = fetched
                .OrderBy(e => e.CreatedAt)
                .ToList();

            Assert.Empty(actual);
        }
        finally
        {
            Context?.Dispose(); 
        }
    }

    protected override WifiReviewRepository GetRepository(TestDbContext context)
    {
        var services = new ServiceCollection();
        services.AddSingleton(context.Database);
        services.AddScoped<WifiReviewRepository>();
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<WifiReviewRepository>();
    }

    private static void AssertWifiReviewValuesEqual(WifiReview expected, WifiReview actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.UserId, actual.UserId);
        Assert.Equal(expected.Text, actual.Text);
        Assert.Equal(expected.Rating, actual.Rating);   
    }
}