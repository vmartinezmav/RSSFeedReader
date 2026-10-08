<!--
Sync Impact Report
Version change: unversioned template -> 1.0.0 (initial project-specific constitution)
Modified principles: template placeholders replaced with five project-specific principles:
	I. MVP Scope and Simplicity
	II. Security and Data Handling
	III. Maintainable Separation
	IV. Verified Behavior and Code Quality
	V. Incremental, Cross-Platform Delivery
Added sections: Technology and Security Constraints; Development Workflow and Quality Gates.
Removed sections: None.
Follow-up TODO: Confirm the original ratification date; it is not recorded in the repository.
-->
# RSS Feed Reader Constitution

## Core Principles

### I. MVP Scope and Simplicity
Implement only behavior approved for the current delivery phase. The MVP is limited to adding
subscriptions by URL and displaying the subscription list; feed fetching, parsing, persistence,
removal, and other deferred capabilities require explicit scope approval. Choose the smallest
implementation that satisfies the approved behavior, and justify any added abstraction or dependency.

### II. Security and Data Handling
Treat user-provided URLs and all remote feed data as untrusted. The MVP MUST NOT make outbound feed
requests. If fetching is approved for a later phase, the implementation MUST restrict permitted URL
schemes and destinations, re-check redirects, bound request time and response size, parse XML
securely, and render remote content as text or sanitize it before HTML rendering. Secrets MUST NOT
be committed to source control or exposed to the browser.

### III. Maintainable Separation
Keep the ASP.NET Core Web API responsible for application and data operations and the Blazor
WebAssembly frontend responsible for user interaction. Communicate through explicit, stable API
contracts; keep environment-specific API addresses and allowed CORS origins in configuration.
Prefer framework facilities and clear, testable components over duplicated logic or abstractions
that do not serve an approved requirement.

### IV. Verified Behavior and Code Quality
Changes MUST include automated tests for changed behavior and relevant regressions. Test API
contracts and service behavior at the appropriate unit or integration boundary. Before a change is
complete, the affected projects MUST build and their relevant tests MUST pass; failures MUST be
resolved or explicitly recorded with an owner and rationale. Review code for correctness,
readability, and consistency with these principles.

### V. Incremental, Cross-Platform Delivery
Deliver and verify the application in small increments that preserve the approved MVP boundary.
Development and test workflows MUST avoid operating-system-specific assumptions where practical,
and the frontend API configuration, backend listening address, and CORS policy MUST agree in each
environment. Record deferred work rather than implementing it implicitly.

## Technology and Security Constraints

The project uses an ASP.NET Core Web API backend and a Blazor WebAssembly frontend. For the MVP,
subscriptions may be held in memory, and the application MUST provide adding a URL and listing
subscriptions without fetching or parsing feeds. Frontend-to-backend communication MUST use the
configured API base address; backend CORS MUST allow the configured frontend origin rather than
relying on mismatched or implicit local ports.

When feed fetching is approved, requests MUST defend against server-side request forgery, including
loopback, private, link-local, and otherwise non-public destinations, including after redirects.
Network operations MUST use bounded timeouts and response sizes. XML parsing MUST disable unsafe
external entity resolution. Feed-provided markup MUST NOT be rendered as trusted HTML.

## Development Workflow and Quality Gates

Each change MUST identify the approved requirement and current delivery phase it serves. Changes
that cross a phase boundary or add a deferred capability require explicit scope approval first.
Before completion, run the build and relevant automated tests for each affected project. Changes
to API behavior require coverage of the affected contract; changes to configuration or cross-origin
communication require checking the corresponding frontend URL and backend CORS settings.

Reviewers MUST check scope, security, maintainability, and test evidence. Any exception MUST be
documented with its rationale, affected scope, and an owner responsible for resolution. Exceptions
do not silently amend this constitution.

## Governance

This constitution governs project design, implementation, and review. Conflicting plans and local
conventions MUST be brought into compliance or explicitly resolved through an approved amendment.
Amendments require a documented rationale, review and approval by the project maintainer(s), an
updated version and amendment date, and a Sync Impact Report describing affected principles,
sections, and follow-up work. Reviews MUST assess compliance with this constitution and record
approved exceptions as described above.

Versioning follows semantic versioning: increment MAJOR for backward-incompatible governance or
principle changes, MINOR for new principles or materially expanded requirements, and PATCH for
clarifications and non-semantic edits.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): confirm original adoption date | **Last Amended**: 2026-10-08
