using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using RSSFeedReader.UI.Pages;
using RSSFeedReader.UI.Services;
using Xunit;

namespace RSSFeedReader.UI.Tests.Pages;

public sealed class SubscriptionsTests
{
    [Fact]
    public async Task Add_displays_exact_values_in_order_and_keeps_duplicates()
    {
        var setup = CreateContext();
        using var context = setup.Context;
        var handler = setup.Handler;
        var component = context.Render<Subscriptions>();
        component.WaitForAssertion(() => Assert.Empty(component.FindAll("li")));

        var values = new[]
        {
            "  https://example.invalid/feed  ",
            "https://example.invalid/second",
            "  https://example.invalid/feed  "
        };

        foreach (var value in values)
        {
            await component.Find("#feed-url").ChangeAsync(value);
            await component.Find("#add-subscription").ClickAsync();
        }

        Assert.Equal(values, component.FindAll("li").Select(item => item.TextContent));
        Assert.Equal(values, handler.Subscriptions);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" \t\r\n")]
    public async Task Add_ignores_blank_or_whitespace_only_values(string value)
    {
        var setup = CreateContext();
        using var context = setup.Context;
        var handler = setup.Handler;
        var component = context.Render<Subscriptions>();
        component.WaitForAssertion(() => Assert.Empty(component.FindAll("li")));

        await component.Find("#feed-url").ChangeAsync(value);
        await component.Find("#add-subscription").ClickAsync();

        Assert.Empty(handler.Subscriptions);
        Assert.Empty(component.FindAll("li"));
    }

    private static (BunitContext Context, SubscriptionHandler Handler) CreateContext()
    {
        var context = new BunitContext();
        var handler = new SubscriptionHandler();
        context.Services.AddScoped(_ => new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5151/api/")
        });
        context.Services.AddScoped<SubscriptionClient>();
        return (context, handler);
    }

    private sealed class SubscriptionHandler : HttpMessageHandler
    {
        public List<string> Subscriptions { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(Subscriptions.ToArray())
                };
            }

            if (request.Method == HttpMethod.Post)
            {
                var json = await request.Content!.ReadAsStringAsync(cancellationToken);
                using var document = JsonDocument.Parse(json);
                Subscriptions.Add(document.RootElement.GetProperty("url").GetString()!);
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
    }
}