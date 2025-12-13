using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

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
        if (itemCount == 0)
        {
            Console.WriteLine("Seeding initial data into SearchDB...");

            var itemData = await File.ReadAllTextAsync("Data/auctions.json");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

            if (items != null && items.Count > 0)
            {
                await DB.SaveAsync(items);
            }
        }
    }
}