# DoseLab Test Strategy

## 1. Scope

**In scope:** `TaperScheduleCalculator` (taper math), `TaperPlanService` (validation, orchestration), `TaperPlansController` (HTTP contract, API key), and the React `App.tsx` (form, preview, plan list).

**Out of scope:** the in-memory repository and catalog internals (test doubles), browser/OS compatibility beyond one modern browser, and real medical correctness of the fictional medications and limits.

## 2. Risks

Ranked by impact times likelihood.

1. **Wrong dose math.** The step is floored to 0.01 mg, the last week keeps the remainder, and the taper never reaches 0. A rounding or off-by-one error means a wrong dose shown for a medical tool.
2. **Validation gaps or masking.** `ValidateDose` reports only the first failure and checks the medication first, so a bad code hides a bad week count. A wrong boundary (0, 1, 52, 53, max dose) lets invalid plans be saved.
3. **Unprotected or mis-mapped API.** `Create` depends on `[RequireApiKey]`, and `TaperValidationException` must become `400`, not `500`. A missing key check allows anonymous writes.
4. **UI and API drift.** The UI only checks "digits" for weeks, so range rules come from the server. A contract change (field names, error shape) silently breaks the form.
5. **Stale preview saved (extra risk).** `schedule` is not cleared when inputs change, so a user can preview one dose, edit it, and save while still seeing the old table.
6. **Unauthenticated read and preview endpoints.** Only `Create` requires an API key. `GET /api/taper-plans`, `GET /api/taper-plans/{id}` and POST `/api/taper-plans/preview` can be called by anyone who knows the URL, bypassing the UI.

## 3. Levels and types

| Level | What we test | Risks |
|---|---|---|
| Unit (xUnit) | Calculator `[Theory]` on even and uneven splits (40/4, 10/3, 1 week); service boundaries 0, 1, 52, 53, dose below 1 and above max; each validation message | 1, 2 |
| Integration (WebApplicationFactory) | `POST` returns `201` with `Location`; invalid input returns `400` with message; unknown id returns `404`; missing or wrong API key rejected | 2, 3 |
| UI (React Testing Library, Playwright) | Component: client errors, preview renders rows, changing inputs clears the schedule. Playwright: one end-to-end preview and save | 4, 5 |
| Performance (k6) | `GET` list and `POST preview` at expected load, P95 and P99 against SLOs | 1 (speed of math) |
| Security | API key required on `Create`; key not logged or exposed in the UI bundle | 3 |

## 4. Tools

xUnit, WebApplicationFactory, React Testing Library, Playwright, k6, CI pipeline (build, test, report).

## 5. Environments and data

- **Local and CI:** API started in-process by WebApplicationFactory with the in-memory repository, so each test starts empty (`TP-0001` is predictable).
- **Data:** created inside each test, never shared. Nothing to clean up because the store is discarded with the host.
- **Playwright and k6:** run against a freshly started API with seeded fictional medications; the instance is thrown away after the run.

## 6. Entry and exit criteria

**Entry:** code builds, the API starts, and the fictional medication catalog is available.

**Done:** all tests green, boundary tests exist for any changed rule, no open high-severity bugs, and every fixed bug has a regression test.

## 7. CI gates

- **Blocks a merge:** build, lint, unit and integration tests, React component tests.
- **Nightly:** Playwright end-to-end, k6 load test against SLOs, dependency and security scan.

## 8. Metrics

Pipeline time, flaky test rate, escaped defects (found after release), defects found per level. Code coverage is a hint, not a goal.