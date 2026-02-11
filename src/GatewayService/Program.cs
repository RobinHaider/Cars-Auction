var builder = WebApplication.CreateBuilder(args);

// add reverse proxy to the service collection
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// add reverse proxy to the request pipeline
app.MapReverseProxy();

app.Run();
