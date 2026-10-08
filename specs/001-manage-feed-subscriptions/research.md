# Research: Manage Feed Subscriptions

## Decisions

### Runtime and project framework

- **Decision**: Target `net10.0` for both projects.
- **Rationale**: The environment has .NET SDK `10.0.204` installed. Microsoft's support table
  identifies .NET 10 as the current LTS release, supported through 2028-11-14; .NET 9 is STS and
  reaches end of support on 2026-11-10. This gives the new project a supported baseline beyond its
  initial MVP period.
- **Alternatives considered**: `net9.0` is installed but is near end of support. A multi-targeted
  project adds no value for this single local application.

### Backend API and state

- **Decision**: Use a small ASP.NET Core API with `GET /api/subscriptions` and
  `POST /api/subscriptions`. Keep an ordered `List<string>` in a singleton in-memory store and
  synchronize reads and writes. The API accepts exact non-whitespace input, retains duplicates,
  and clears all values when the API process stops.
- **Rationale**: This matches the specified add/list behavior, preserves insertion order and exact
  values, and avoids persistence, URL parsing, feed requests, and unnecessary abstractions. A
  synchronized store keeps snapshots stable if requests overlap.
- **Alternatives considered**: A database violates the session-only MVP. A feed client/parser is
  explicitly out of scope. Controllers and a repository layer add ceremony without another API
  use case. Browser-only state would bypass the project-selected API-owned data flow.

### API contract

- **Decision**: `GET` returns an ordered JSON array of strings with `200 OK`. `POST` accepts
  `{"url":"<submitted value>"}` and returns `204 No Content` when accepted. Missing, empty, or
  whitespace-only values return `400 Bad Request` and do not change the list.
- **Rationale**: The response does not invent a subscription identifier when duplicates are valid
  and no item-level operations exist. The contract permits the UI to append the accepted value
  locally without an additional round trip.
- **Alternatives considered**: Returning a created resource and `201 Created` implies a stable
  individually addressable subscription that this MVP does not define.

### Blazor interaction

- **Decision**: Provide a single subscription page with a plain text input, an Add action, and an
  ordered list. Load the existing list when the page opens, submit only non-whitespace values, and
  append the exact value after a successful response. Render URLs as text.
- **Rationale**: A URL-specific browser input may impose format checks contrary to the spec. Plain
  text preserves the requirement to accept arbitrary non-empty strings, and normal text rendering
  avoids treating user input as markup.
- **Alternatives considered**: Client-side URL parsing, trimming, canonicalization, deduplication,
  and feed preview all change specified behavior or add deferred features.

### Session lifecycle

- **Decision**: Treat one local application run as the API process lifetime. Start the UI and API
  together; stopping the application stops the API, which clears the list. Reloading only the UI
  while leaving the API running does not end the session.
- **Rationale**: The chosen architecture assigns data operations to the API, and process-local
  memory naturally models its lifecycle without per-browser session identifiers or cleanup logic.
- **Alternatives considered**: Browser-scoped server sessions require identifiers, session
  middleware, and expiration behavior not needed by the single-user local MVP.

### Testing

- **Decision**: Use xUnit for backend store and API integration tests, `Microsoft.AspNetCore.Mvc.Testing`
  to exercise HTTP contracts, and bUnit for the Blazor component. Keep the end-to-end run-through in
  `quickstart.md`.
- **Rationale**: These checks cover behavior at the API and UI boundaries while remaining compatible
  with the constitution's automated-test requirement.
- **Alternatives considered**: Manual-only verification would not satisfy the project's quality
  gate. Browser automation is not required for this small interaction if component and API tests
  cover it.

## Resolved Unknowns

- Framework version: `net10.0`, based on installed SDK and LTS lifecycle.
- Session boundary: API process lifetime; the local app's stop action terminates that process.
- API request/response behavior: defined in [contracts/subscriptions.md](contracts/subscriptions.md).
- No unresolved technical-context questions remain for this MVP.

## Sources

- [Microsoft .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core),
  last updated 2026-09-08.
- [Project goals](../../.github/StakeholderDocuments/ProjectGoals.md).
- [Technology stack](../../.github/StakeholderDocuments/TechStack.md).
- [Feature specification](spec.md) and [project constitution](../../.specify/memory/constitution.md).
- Environment check: `dotnet --list-sdks` reported `9.0.318` and `10.0.204`.