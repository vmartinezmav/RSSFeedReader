# Subscription API Contract

The API is used by the local Blazor frontend to add and list subscriptions. These operations do
not contact the submitted feed URLs. The API is intended for the local application; its base
address and allowed frontend origin are configuration, not hard-coded assumptions.

## `GET /api/subscriptions`

Returns the current process's subscriptions in submission order.

- Success: `200 OK`, `Content-Type: application/json`
- Body: JSON array of strings; an empty list is `[]`
- Duplicate values are returned as separate elements.

## `POST /api/subscriptions`

Adds one subscription value.

- Request: `Content-Type: application/json`
- Body: `{"url":"https://example.invalid/feed"}`
- Success: `204 No Content`; append the exact value to the current list.
- Invalid request: `400 Bad Request` for a missing, empty, or whitespace-only `url`; do not
  mutate the list.
- Do not trim, parse, normalize, check reachability, or request the submitted URL.

## Lifecycle and access

- The list is scoped to the API process and is lost when that process stops.
- The local application starts and stops the UI and API together; a UI reload alone does not
  restart the API or clear its list.
- Configure CORS to allow the frontend's configured local origin. Do not use wildcard origins.
- Authentication and cross-user isolation are outside the single-user local MVP.