using System.Net.Http.Json;

namespace RSSFeedReader.UI.Services;

public sealed class SubscriptionClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<string>> GetSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<List<string>>("subscriptions", cancellationToken) ?? [];
    }

    public async Task AddSubscriptionAsync(string url, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "subscriptions",
            new AddSubscriptionRequest(url),
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private sealed record AddSubscriptionRequest(string Url);
}