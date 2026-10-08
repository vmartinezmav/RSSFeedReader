using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RSSFeedReader.Api.Tests.Subscriptions;

public sealed class SubscriptionsApiTests
{
    [Fact]
    public async Task Get_returns_empty_list_for_a_fresh_process()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/subscriptions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty((await response.Content.ReadFromJsonAsync<string[]>())!);
    }

    [Fact]
    public async Task Post_adds_exact_value_and_get_returns_it()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        const string url = "  https://example.invalid/feed  ";

        using var postResponse = await client.PostAsJsonAsync("/api/subscriptions", new { url });
        using var getResponse = await client.GetAsync("/api/subscriptions");

        Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
        Assert.Equal(new[] { url }, await getResponse.Content.ReadFromJsonAsync<string[]>());
    }

    [Fact]
    public async Task Post_preserves_duplicate_values_in_submission_order()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        var urls = new[] { "https://example.invalid/one", "https://example.invalid/one", "not a URL" };

        foreach (var url in urls)
        {
            using var response = await client.PostAsJsonAsync("/api/subscriptions", new { url });
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        var actual = await client.GetFromJsonAsync<string[]>("/api/subscriptions");

        Assert.Equal(urls, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\r\n")]
    public async Task Post_rejects_missing_or_whitespace_url_without_mutation(string? url)
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var postResponse = await client.PostAsJsonAsync("/api/subscriptions", new { url });
        var subscriptions = await client.GetFromJsonAsync<string[]>("/api/subscriptions");

        Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        Assert.Empty(subscriptions!);
    }

    [Fact]
    public async Task Post_rejects_request_without_url_property()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var postResponse = await client.PostAsJsonAsync("/api/subscriptions", new { other = "value" });
        var subscriptions = await client.GetFromJsonAsync<string[]>("/api/subscriptions");

        Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        Assert.Empty(subscriptions!);
    }

    [Fact]
    public async Task New_host_starts_with_empty_list_after_previous_host_stops()
    {
        using (var previousFactory = new WebApplicationFactory<Program>())
        using (var previousClient = previousFactory.CreateClient())
        using (var response = await previousClient.PostAsJsonAsync(
            "/api/subscriptions",
            new { url = "https://example.invalid/feed" }))
        {
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        using var newFactory = new WebApplicationFactory<Program>();
        using var newClient = newFactory.CreateClient();

        Assert.Empty((await newClient.GetFromJsonAsync<string[]>("/api/subscriptions"))!);
    }
}