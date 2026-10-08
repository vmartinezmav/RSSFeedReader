# RSS Feed Reader

A small, local RSS/Atom subscription manager built as an MVP. Add feed URL values and view them in a list; the app does not fetch or display feed content.

## Features

- Add any non-empty value as a subscription.
- Display subscriptions in the order entered, including duplicates.
- Ignore blank and whitespace-only input.
- Keep subscriptions in memory for the lifetime of the API process. Restarting the API clears the list.

## Technology

- ASP.NET Core Web API (`net10.0`)
- Blazor WebAssembly (`net10.0`)
- xUnit API integration tests and bUnit UI component tests

## Configuration

The API listens on `http://localhost:5151`; the UI listens on `http://localhost:5213`. The UI's API base address is set in `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`, and the API's allowed UI origin is set in `backend/RSSFeedReader.Api/appsettings.json`. Keep these values aligned when changing local ports.

## Tests

Run all API and UI tests from the repository root:

```powershell
dotnet test RSSFeedReader.sln -m:1
```

## Run the project from a terminal

Open two terminals at the repository root. Start the API in the first terminal:

```powershell
dotnet run --project backend/RSSFeedReader.Api
```

Start the UI in the second terminal:

```powershell
dotnet run --project frontend/RSSFeedReader.UI
```

The UI opens at [http://localhost:5213](http://localhost:5213).
