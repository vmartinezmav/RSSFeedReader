using RSSFeedReader.Api.Subscriptions;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins(allowedOrigins)
            .WithMethods("GET", "POST")
            .AllowAnyHeader());
});
builder.Services.AddSingleton<InMemorySubscriptionStore>();

var app = builder.Build();

app.UseCors();

app.MapGet("/api/subscriptions", (InMemorySubscriptionStore store) =>
    Results.Ok(store.GetAll()));

app.MapPost("/api/subscriptions", (AddSubscriptionRequest? request, InMemorySubscriptionStore store) =>
{
    return store.TryAdd(request?.Url)
        ? Results.NoContent()
        : Results.BadRequest();
});

app.Run();

public partial class Program;
