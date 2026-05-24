# English Definitions Review Workflow Spec

- Date: 2026-05-10
- Status: Draft
- Repository: YorubaNameDictionary
- Primary scope in this repository: API and Words Website
- Context-only sections in this document: Dashboard and Migration (captured for cross-system alignment)

## 1. Purpose

This specification introduces an English definitions workflow that allows new English definitions to be captured without immediate public display, then reviewed and approved before being shown on the Words website.

The workflow is designed to:
- Preserve existing definitions and behavior for current data.
- Support fetching multiple existing English definitions per word.
- Support creating proposed definitions that require review.
- Ensure website display excludes unreviewed definitions.
- Keep Dashboard and Migration requirements visible for downstream implementation in related systems.

## 2. Scope

### In scope for this repository
- API changes for definition review metadata and English-definitions endpoints.
- Words Website display rule update to hide unreviewed definitions.
- Specification folder setup for future spec-driven development.

### Out of direct implementation scope for this repository (context only)
- Names Dashboard changes.
- Words Dashboard editor enhancements.
- Migration execution from Yoruba Name and Yoruba Word dictionary etymologies.

## 3. Domain Model Changes (API)

### Definition object
Add field:
- `NeedsReview: boolean | null`

Behavior:
- Existing definitions remain unchanged and can have `NeedsReview = null`.
- New definitions created through the new POST endpoint must set `NeedsReview = true`.
- Reviewed definitions (set by dashboard/editor flows outside this repo scope) are expected to move to `NeedsReview = false`.

Compatibility:
- No manual migration required for existing records.
- Null must be treated as legacy state.

## 4. API Requirements

Base path prefix is existing API versioning (`v1`).

### 4.1 GET `v1/words/definitions/in-english?q={array-of-words}`

#### Intent
Return all existing English definitions for each requested word, regardless of `NeedsReview` status.

#### Request
- Method: `GET`
- Route: `v1/words/definitions/in-english`
- Query param:
  - `q`: array of words

#### Response
- `200 OK` with dictionary/object:
  - key: word
  - value: array of English definitions

Example response:
```json
{
  "omo": ["child", "offspring", "human being"],
  "ife": ["love", "affection"]
}
```

Notes:
- The response includes definitions irrespective of `NeedsReview` (`null`, `true`, `false`).
- If a word is unknown, return an empty array for that word.
- Word matching/casing/normalization should follow existing API word lookup behavior.
- This endpoint should follow the same query-driven lookup pattern used by the existing `latest-meaning` endpoint in `Api.Controllers.Names.EtymologyController`.

### 4.2 POST `v1/words/definitions/in-english`

#### Intent
Create new English definitions that do not already exist for each supplied word, and mark each created definition as review-required.

#### Request
- Method: `POST`
- Route: `v1/words/definitions/in-english`
- Body: dictionary/object where:
  - key: word
  - value: new English definition (string)

Example request:
```json
{
  "ife": "deep fondness",
  "iwa": "character disposition"
}
```

#### Behavior
- For each `(word, definition)` pair:
  - If the definition does not exist for the word, create it with `NeedsReview = true`.
  - If the definition already exists, do not duplicate it.
- Endpoint is complementary to the GET endpoint and uses the same word semantics.

#### Response
- `200 OK` or `201 Created` based on existing API conventions.
- Should include per-word outcome details (created, skipped-duplicate, not-found) to support dashboard workflows.

Suggested response shape:
```json
{
  "ife": { "status": "created" },
  "iwa": { "status": "created" },
  "omo": { "status": "skipped-duplicate" }
}
```

## 5. Words Website Requirements

Display rule update:
- Only definitions where `NeedsReview != true` are shown on the website.

Handling legacy records:
- Definitions with `NeedsReview = null` are legacy values.
- This spec follows the explicit requirement: only `false` is displayable.
- If legacy null values currently appear in production, product alignment is required before rollout to avoid unintended content hiding.

## 6. Dashboard Requirements (Context Only)

These requirements are documented for dependent systems and integrations:

1. Add page listing words that contain at least one definition with review-needed state.
2. Names dashboard etymology meaning flow moves from latest-meaning lookup to new english-definitions endpoint:
- User can cycle through returned definitions for each etymology part.
- Manual etymology part entry auto-fetches definitions and auto-selects first definition.
- Glossary-generated etymology parts fetch definitions in batch and auto-select first value per part.
3. If user enters a meaning not in existing definitions, submit via POST `v1/words/definitions/in-english`.
4. Words dashboard should populate etymology meaning using the same endpoint.
5. Names and Words editor create/edit:
- For any definition with `NeedsReview = true`, show `Reviewed` checkbox.
- Do not show checkbox for `NeedsReview = null` or `false`.
- Form cannot be submitted until all shown `Reviewed` checkboxes are checked.
6. Before publish, show final warning listing reviewed definitions that will become publicly visible.
7. Extend etymology population behavior to create page in both dashboards.

## 7. Migration Requirements (Context Only)

Apply the same migration pattern to:
- Yoruba Name dictionary etymologies
- Yoruba Word dictionary etymologies

Algorithm:
1. Existing word:
- Add new definition with English translation value.
- Mark added definition as review-needed (not publicly visible until reviewed/expanded).
- Do not unpublish word.
- Tag word with `Definition(s) Need Review`.
2. New word:
- Create new word entry with `state = NEW`, `partOfSpeech = UNKNOWN`.
- Include migrated definition.
- Tag with `Definitions Need Review`.
3. Preserve all existing definitions.
4. Deduplicate English translations.

## 8. Acceptance Criteria

### API
1. Existing records remain valid without migration scripts.
2. GET endpoint returns dictionary mapping each requested word to all English definitions regardless of review status.
3. POST endpoint creates only non-existing definitions and sets `NeedsReview = true` for created entries.
4. Duplicate submissions do not create duplicate definitions.

### Words Website
1. Website renders only definitions with `NeedsReview = false`.
2. Definitions with `NeedsReview = true` are never displayed publicly.
3. Behavior for `NeedsReview = null` follows explicit product decision documented in this spec.

## 9. Testing Guidance

### API tests
- GET endpoint:
  - returns all statuses (`null`, `true`, `false`) in response values.
  - handles unknown words with empty arrays.
- POST endpoint:
  - creates new definition with `NeedsReview = true`.
  - skips duplicate definitions.
  - handles mixed outcomes in one request.

### Website tests
- Confirm only `NeedsReview = false` definitions render.
- Confirm `true` and `null` definitions are excluded.

## 10. Open Questions

1. Query serialization for `q` array in GET endpoint:
- Repeated query keys (`?q=ife&q=iwa`) vs comma-separated (`?q=ife, iwa`) vs JSON array string.
2. Response contract for POST outcomes:
- Final status code and exact payload schema.
3. Should unknown words in POST auto-create words or return not-found (current spec assumes outcome reporting without enforcing creation).
4. Final product confirmation on website behavior for legacy `NeedsReview = null` definitions.

## 11. Future Spec-Driven Development Convention

This repository will store new product/engineering specs under:
- `specs/`

Recommended filename format:
- `YYYY-MM-DD-short-feature-name-spec.md` for time-based traceability, or
- `short-feature-name-spec.md` for stable long-lived specs.
