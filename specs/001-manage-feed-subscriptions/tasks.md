---

description: "Implementation task list for Manage Feed Subscriptions"
---

# Tasks: Manage Feed Subscriptions

**Input**: Design documents from `specs/001-manage-feed-subscriptions/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Tests**: Automated API and Blazor component tests are required by the project constitution.

**Organization**: Tasks are grouped by user story so the MVP can be independently implemented and tested.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: The task can run in parallel with another marked task because files and dependencies do not overlap.
- **[Story]**: User story served by the task; setup, foundation, and polish tasks have no story label.
- Every task includes the concrete file path(s) to create or update.

## Phase 1: Setup

**Purpose**: Create the solution, application projects, and test projects required by the plan.

- [X] T001 Create the solution file `RSSFeedReader.sln` at the repository root.
- [X] T002 Create the ASP.NET Core Web API project targeting `net10.0` in `backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj` and add it to `RSSFeedReader.sln`.
- [X] T003 Create the Blazor WebAssembly project targeting `net10.0` in `frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj` and add it to `RSSFeedReader.sln`.
- [X] T004 Create the API test project with xUnit and `Microsoft.AspNetCore.Mvc.Testing` in `backend/RSSFeedReader.Api.Tests/RSSFeedReader.Api.Tests.csproj`, referencing `backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj` and `RSSFeedReader.sln`.
- [X] T005 Create the Blazor component test project with xUnit and bUnit in `frontend/RSSFeedReader.UI.Tests/RSSFeedReader.UI.Tests.csproj`, referencing `frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj` and `RSSFeedReader.sln`.

---

## Phase 2: Foundational

**Purpose**: Remove generated route conflicts and establish matching local origins before implementing the user story.

- [X] T006 [P] Remove the template pages `frontend/RSSFeedReader.UI/Pages/Home.razor`, `frontend/RSSFeedReader.UI/Pages/Counter.razor`, and `frontend/RSSFeedReader.UI/Pages/Weather.razor`; update `frontend/RSSFeedReader.UI/Layout/NavMenu.razor` and `frontend/RSSFeedReader.UI/Layout/MainLayout.razor` to expose only the subscriptions experience.
- [X] T007 [P] Configure API and UI local HTTP ports and the API base URL in `backend/RSSFeedReader.Api/Properties/launchSettings.json`, `frontend/RSSFeedReader.UI/Properties/launchSettings.json`, and `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` to match the ports and `/api/` base path documented in `quickstart.md`.

**Checkpoint**: Both projects and test projects build; the root route is unambiguous and local API/UI addresses agree.

---

## Phase 3: User Story 1 - Add and View Feed Subscriptions (Priority: P1)

**Goal**: Let one local user add non-empty feed URL strings and immediately view the exact values in order, including duplicates, for the lifetime of the API process.

**Independent Test**: With the local application running, submit a non-empty value and verify that it appears exactly once; submit it again and verify two ordered entries; verify blank input adds nothing; restart the API and verify an empty list. No submitted address is contacted.

### Tests for User Story 1

- [X] T008 [P] [US1] Add API integration tests for empty-list GET, successful POST, exact value preservation, duplicate ordering, blank/missing URL rejection without mutation, and empty state after a fresh API process in `backend/RSSFeedReader.Api.Tests/Subscriptions/SubscriptionsApiTests.cs`.
- [X] T009 [P] [US1] Add Blazor component tests for submitting a non-whitespace value, displaying the accepted value, retaining duplicate entries, and ignoring blank or whitespace-only input in `frontend/RSSFeedReader.UI.Tests/Pages/SubscriptionsTests.cs`.

### Implementation for User Story 1

- [X] T010 [P] [US1] Implement the process-local ordered subscription list and synchronized add/list operations in `backend/RSSFeedReader.Api/Subscriptions/InMemorySubscriptionStore.cs`; reject whitespace-only values without mutation and preserve all other input exactly, including duplicates.
- [X] T011 [P] [US1] Implement the typed frontend HTTP client for `GET /api/subscriptions` and `POST /api/subscriptions` in `frontend/RSSFeedReader.UI/Services/SubscriptionClient.cs` and register it in `frontend/RSSFeedReader.UI/Program.cs`; read the API base address from `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`.
- [X] T012 [US1] Register the singleton store, configure CORS for the configured local UI origin, and map `GET /api/subscriptions` plus `POST /api/subscriptions` with the documented `200`, `204`, and `400` responses in `backend/RSSFeedReader.Api/Program.cs`; define the nullable request URL in `backend/RSSFeedReader.Api/Subscriptions/AddSubscriptionRequest.cs`.
- [X] T013 [US1] Implement the subscription input and ordered list in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`; use a plain text input, ignore blank or whitespace-only values, preserve accepted values exactly, allow duplicates, and render values as text.

**Checkpoint**: User Story 1 works and passes its API and component tests independently.

---

## Phase 4: Polish and Cross-Cutting Validation

**Purpose**: Verify the complete MVP against the documented acceptance scenarios without adding deferred features.

- [X] T014 Run both test projects and complete every acceptance check in `specs/001-manage-feed-subscriptions/quickstart.md`; verify the application builds, local origins and CORS agree, and no request is sent to a submitted feed URL.

---

## Dependencies and Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Run T001 through T005 in order because each project task updates the shared solution file; T004 follows both application projects and T005 follows T004.
- **Foundational (Phase 2)**: T006 and T007 follow project creation and can run in parallel; both block user-story work.
- **User Story 1 (Phase 3)**: Depends on Phase 2. T008 and T009 can run in parallel before implementation. T010 and T011 can then run in parallel; T012 depends on T010, and T013 depends on T011.
- **Polish (Phase 4)**: T014 depends on all User Story 1 tasks.

### User Story Dependencies

- **User Story 1 (P1)**: Depends only on Setup and Foundational phases; it is the entire MVP and has no dependency on another user story.

### Parallel Opportunities

- T006 and T007 update separate route and configuration files.
- T008 and T009 cover separate API and UI test files.
- T010 and T011 implement separate backend and frontend files; their dependent endpoint and page tasks follow independently.

### Parallel Example: User Story 1

```text
After Phase 2:
  T008 API integration tests       || T009 Blazor component tests
  T010 in-memory API store          || T011 frontend API client
After T010: T012 API endpoints
After T011: T013 subscriptions page
```

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational phases, including verification that only the subscriptions page owns the root route.
2. Complete User Story 1, preserving the API-process session boundary and the exact input, duplicate, and ordering rules.
3. Run both automated test projects and the independent quickstart acceptance checks.
4. Stop at the MVP boundary; feed fetching, parsing, persistence, removal, and item display are deferred.

### Incremental Delivery

The project contains one user story. Deliver the add/list API and UI together as that independently testable increment; do not create additional phases for deferred product capabilities.