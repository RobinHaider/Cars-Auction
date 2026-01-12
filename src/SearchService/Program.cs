using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;
using MassTransit;
using SearchService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetRetryPolicy());

// Register MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Register the consumer
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    // Use Kebab Case for endpoint names
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    // Configure RabbitMQ
    x.UsingRabbitMq((context, config) =>
    {
        config.Host("localhost", "/", h =>
        {
            h.Username("dev");
            h.Password("dev123");
        });

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
