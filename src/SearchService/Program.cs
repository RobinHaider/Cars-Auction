using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetRetryPolicy());

// Register MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, config) =>
    {
        config.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

// Initialize the database on startup 
app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
});

app.Run();

// Define the retry policy: retry on transient errors with a delay
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
   => HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
