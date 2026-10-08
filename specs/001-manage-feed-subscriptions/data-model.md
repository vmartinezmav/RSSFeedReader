# Data Model: Manage Feed Subscriptions

## Feed Subscription

Represents one non-empty value submitted by the user. The MVP manages the submitted URL string;
it does not resolve, validate, or fetch the URL.

| Attribute | Meaning | Rules |
|---|---|---|
| `url` | The value entered by the user | Must not be empty or whitespace-only; otherwise preserved exactly as entered |
| list position | The order in which the value was accepted | Assigned by append order; not supplied or changed by the user |

There is no unique identifier in this MVP. Repeated URL values are separate entries and remain
separate in the list. An API process restart empties the list; no subscription survives beyond that
process lifetime.

## State Changes

| Event | Result |
|---|---|
| Submit a non-whitespace value | Append the exact value to the list |
| Submit an empty or whitespace-only value | Reject without changing the list |
| Read the list | Return a snapshot in append order |
| Stop and restart the API process | Start with an empty list |

No feed-item, user-account, persistence, or feed-fetching entities are part of this feature.