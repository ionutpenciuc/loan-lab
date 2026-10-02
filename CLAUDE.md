# Dose lab — agent context

A practice app for QA automation. .NET 10 API (`backend/`) + React/Vite UI (`frontend/`). Data is in memory.

## Rules

- Do not read or use anything in `instructor/`. It holds exam answers and hidden bugs.
- Never change code in `backend/src/` or `frontend/src/` to make a test pass. If the code looks wrong, report it as a bug.
- Write a test from the **rule** and the expected values, not from the current code.
- Every new test must be seen failing once (break the expected value, run, put it back).
- Use synthetic data only. Never real patient data.

## Commands

| Task | Command | Folder |
| --- | --- | --- |
| Unit + API tests | `dotnet test` | `backend` |
| Start API (port 5080) | `dotnet run --project src/DoseLab.Api` | `backend` |
| Start UI (port 5173) | `npm run dev` | `frontend` |
| Playwright | `npm run test:e2e` | `frontend` |
| k6 smoke | `k6 run perf/smoke.js` | repo root (API running) |

## Test conventions

- C#: xUnit. Class `{Subject}Should`. Method name finishes the sentence (`RejectAnUnknownMedication`). Body has `// setup`, `// execute`, `// verify`, each followed by one empty line.
- Unit tests: `backend/tests/DoseLab.UnitTests/`. Use `SimulatedLatency.None` and a fixed `TimeProvider`.
- API tests: `backend/tests/DoseLab.ApiTests/`, with `WebApplicationFactory<Program>`. Do not assert exact list counts: the app is shared.
- Playwright: `frontend/e2e/`. Role and label locators. Web-first assertions. No `waitForTimeout`. Unique names for saved data.
- k6: `perf/`. Always set thresholds.

## Domain rules

- Medications: STR Steriva max 80 mg, CLM Calmafen max 40 mg, NRV Nervalin max 20 mg (fictional).
- Starting dose: 1 mg to the medication maximum, inclusive. Weeks: 1 to 52.
- Step = starting dose ÷ weeks, rounded down to 0.01 mg. Week n dose = start − (n − 1) × step. Weekly total = daily × 7.
- `POST /api/taper-plans` needs header `X-Api-Key: lab-dev-key`. Success is 201 with a `Location` header.
- Reference numbers are `TP-0001`, `TP-0002`, … and must be unique.
