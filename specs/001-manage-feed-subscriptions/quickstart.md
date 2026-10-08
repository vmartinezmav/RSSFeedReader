# Quickstart: Manage Feed Subscriptions

## Prerequisites

- .NET 10 SDK and a current browser.
- The implementation projects and local API/UI settings described in
  [plan.md](plan.md) and [subscriptions.md](contracts/subscriptions.md).
- Restore/build dependencies before running the offline acceptance check.

## Build and automated tests

From the repository root, after implementation creates the planned projects:

```powershell
dotnet test backend/RSSFeedReader.Api.Tests/RSSFeedReader.Api.Tests.csproj
dotnet test frontend/RSSFeedReader.UI.Tests/RSSFeedReader.UI.Tests.csproj
```

Expected result: both commands succeed; backend tests verify blank rejection, exact value
preservation, duplicates, ordering, process-local reset behavior, and the HTTP contract. Component
tests verify submission and list updates, including blank input behavior.

## Run the local application

Start the API in one terminal:

```powershell
dotnet run --project backend/RSSFeedReader.Api -- --urls http://localhost:5151
```

Start the UI in a second terminal:

```powershell
dotnet run --project frontend/RSSFeedReader.UI -- --urls http://localhost:5213
```

Open `http://localhost:5213`. The frontend API address must point to `http://localhost:5151/api/`,
and the API CORS policy must allow `http://localhost:5213`.

## Acceptance checks

1. Submit `https://example.invalid/feed`. It appears exactly as entered without a request to that
   address.
2. Submit the same value again. Two entries appear in submission order.
3. Submit an invalid-looking but non-empty string. It appears unchanged; no URL validation or feed
   request occurs.
4. Submit blank and whitespace-only input. No entry is added.
5. Disable external network access while retaining local loopback, then repeat the add/list checks.
   Subscription management continues to work; only local UI-to-API traffic occurs.
6. Stop both the UI and API processes, restart them, and verify that the list is empty.

The application should show the list update within one second of each accepted submission. No feed
items, persistence, remove action, or background refresh should be present in this MVP.