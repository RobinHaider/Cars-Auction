using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication application)
    {
        await DB.InitAsync(
            "SearchDB",
            MongoClientSettings.FromConnectionString(application.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        // seed data if necessary
        var itemCount = await DB.CountAsync<Item>();

        // only seed if no items exist
        using var scope = application.Services.CreateScope();
        var auctionClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

        var items = await auctionClient.GetItemsForSearchDb();

        Console.WriteLine("returned from auction service", items.Count);

        if (items.Count > 0) await DB.SaveAsync(items);

    }
}