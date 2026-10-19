# Dose lab — agent context

A practice app for QA automation. .NET 10 API (`backend/`) + React/Vite UI (`frontend/`) + Windows Forms app (`desktop/`, Windows only). Data is in memory by default; PostgreSQL with `Lab:Storage=Postgres`.

## Rules

- Do not read or use anything in `instructor/`. It holds exam answers and hidden bugs.
- Never change code in `backend/src/`, `frontend/src/` or `desktop/src/` to make a test pass. If the code looks wrong, report it as a bug.
- Write a test from the **rule** and the expected values, not from the current code.
- Every new test must be seen failing once (break the expected value, run, put it back).
- Use synthetic data only. Never real patient data.

## Commands

| Task | Command | Folder |
| --- | --- | --- |
| Unit + API + integration tests | `dotnet test` (Docker running) | `backend` |
| Without Docker | `dotnet test --filter "Category!=Docker"` | `backend` |
| PostgreSQL for the API | `docker compose up -d`, then `--launch-profile http-postgres` | repo root |
| Desktop UI tests (Windows) | `dotnet test DoseLab.Desktop.slnx` | `desktop` |
| Mutation testing | `dotnet tool restore && dotnet stryker` | `backend/tests/DoseLab.UnitTests` |
| Start API (port 5080) | `dotnet run --project src/DoseLab.Api` | `backend` |
| Start UI (port 5173) | `npm run dev` | `frontend` |
| Playwright | `npm run test:e2e` | `frontend` |
| k6 smoke | `k6 run perf/smoke.js` | repo root (API running) |

## Test conventions

- C#: xUnit. Class `{Subject}Should`. Method name finishes the sentence (`RejectAnUnknownMedication`). Body has `// setup`, `// execute`, `// verify`, each followed by one empty line.
- Unit tests: `backend/tests/DoseLab.UnitTests/`. Use `SimulatedLatency.None` and a fixed `TimeProvider`.
- Assertions: xUnit `Assert` or FluentAssertions 7. Mocks: NSubstitute. Time: `FakeTimeProvider`. Data: `Builders/TaperPlanBuilder`.
- API tests: `backend/tests/DoseLab.ApiTests/`, with `WebApplicationFactory<Program>` or `Harness/DoseLabApiFactory`. Do not assert exact list counts: the app is shared.
- Integration tests: `backend/tests/DoseLab.IntegrationTests/`, Testcontainers, `[Collection(PostgresCollection.Name)]` and `[Trait("Category", "Docker")]`, a new database per test.
- Desktop: `desktop/tests/DoseLab.Desktop.UiTests/`. Find elements by AutomationId through the screen object. No `Thread.Sleep`.
- `TODO(Lnn-n)` marks a lesson exercise for the student. Do not solve exercises unless the student asks for that exercise.
- Playwright: `frontend/e2e/`. Role and label locators. Web-first assertions. No `waitForTimeout`. Unique names for saved data.
- k6: `perf/`. Always set thresholds.

## Domain rules

- Medications: STR Steriva max 80 mg, CLM Calmafen max 40 mg, NRV Nervalin max 20 mg (fictional).
- Starting dose: 1 mg to the medication maximum, inclusive. Weeks: 1 to 52.
- Step = starting dose ÷ weeks, rounded down to 0.01 mg. Week n dose = start − (n − 1) × step. Weekly total = daily × 7.
- `POST /api/taper-plans` needs header `X-Api-Key: lab-dev-key`. Success is 201 with a `Location` header.
- Reference numbers are `TP-0001`, `TP-0002`, … and must be unique.
