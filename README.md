# Dose lab

A small medical-style app for learning test automation: unit, API, integration with a real database, web UI (Playwright), desktop UI (FlaUI), performance and stress (k6, BenchmarkDotNet), CI, penetration testing, and AI-assisted testing.

Nurses create **taper plans**: a patient's daily dose of a medication goes down by the same step every week. The app shows the plans, previews the week-by-week schedule, and saves new plans.

The medications and their limits are **fictional**. This is not medical guidance.

- Backend: .NET 10 Web API, in `backend/`. By default data is kept in memory: restart the API and saved plans are gone, and two sample plans come back. It can also use PostgreSQL (lesson 12).
- Frontend: React + TypeScript (Vite), in `frontend/`.
- Desktop: a small Windows Forms app, in `desktop/` (Windows only, lesson 15).

**Start here after setup: [lessons/README.md](lessons/README.md)**, the 7-day study plan.

## Install the tools

| Tool | Check | Download |
| --- | --- | --- |
| Git | `git --version` | https://git-scm.com |
| .NET 10 SDK | `dotnet --version` (must start with `10.`) | https://dotnet.microsoft.com/download/dotnet/10.0 |
| Node.js 20 or newer | `node --version` | https://nodejs.org |
| k6 (for lesson 05) | `k6 version` | https://grafana.com/docs/k6/latest/set-up/install-k6/ |
| Docker Desktop (from lesson 12) | `docker run --rm hello-world` | [tools/docker-and-postgres.md](tools/docker-and-postgres.md) |

Step-by-step for Windows, and guides for every other tool (ZAP, Burp, Stryker, FlaUI, Ranorex, Cypress, Claude Code): [tools/README.md](tools/README.md).

On Windows, use Git Bash for the `instructor/bug.sh` script. All other commands work in PowerShell too.

## Start the app

### 1. Clone

Clone your own fork (see lessons/README.md), or this repo:

```bash
git clone https://github.com/ionutpenciuc/loan-lab.git
cd loan-lab
```

### 2. Start the API (terminal 1)

```bash
cd backend
dotnet run --project src/DoseLab.Api
```

Wait for `Now listening on: http://localhost:5080`. The first run downloads packages and can take about a minute.

- API docs: http://localhost:5080/swagger
- Ready-made requests for Rider or VS Code: `backend/DoseLab.http`

In Rider or Visual Studio: open `backend/DoseLab.slnx`, choose the `http` launch profile, and press Run.

To keep data in PostgreSQL instead: `docker compose up -d` in the repo folder, then use the `http-postgres` launch profile (`dotnet run --project src/DoseLab.Api --launch-profile http-postgres`).

### 3. Start the UI (terminal 2)

From the repo folder:

```bash
cd frontend
npm install
npm run dev
```

Open http://localhost:5173. You should see two plans: Maria Ionescu and Andrei Stan.

### 4. Try it once

1. Click **Add taper plan**.
2. Patient name: `Ana Pop`. Medication: **Calmafen**. Starting daily dose: `10`. Number of weeks: `3`.
3. Click **Preview schedule**. Week 2 shows `6.67`. Week 3 shows `3.34`.
4. Click **Save**. Ana Pop appears in the list with reference `TP-0003`.

## Run the tests

| Level | Command | Folder |
| --- | --- | --- |
| Unit + API + integration | `dotnet test` (needs Docker running) | `backend` |
| The same, without Docker | `dotnet test --filter "Category!=Docker"` | `backend` |
| UI (Playwright) | `npx playwright install chromium` (once), then `npm run test:e2e` | `frontend` |
| Desktop UI (FlaUI, Windows only) | `dotnet test DoseLab.Desktop.slnx` | `desktop` |
| Performance smoke | `k6 run perf/smoke.js` (API must be running) | repo root |
| Micro-benchmarks | `dotnet run -c Release --project benchmarks/DoseLab.Benchmarks -- --filter "*"` | `backend` |
| Mutation testing | `dotnet tool restore`, then `dotnet stryker` | `backend/tests/DoseLab.UnitTests` |

Skipped tests are exercises: see "Exercises in the code" in [lessons/README.md](lessons/README.md).

Playwright starts the API and the UI itself, or reuses them if they already run. To use other ports: `LAB_API_PORT=5181 LAB_UI_PORT=5182 npm run test:e2e`.

GitHub Actions runs unit, API, integration (with coverage), Playwright, desktop UI and a k6 smoke on every push (`.github/workflows/ci.yml`), and mutation testing plus a ZAP security scan every night (`.github/workflows/nightly.yml`).

## Repo map

| Path | What |
| --- | --- |
| `backend/src/DoseLab.Application/` | Rules: taper calculator, service, in-memory repository, medication catalog |
| `backend/src/DoseLab.Postgres/` | The PostgreSQL repository |
| `backend/src/DoseLab.Algorithms/` | Interview algorithms: search, sort, hash map, stack, graph, dynamic programming |
| `backend/src/DoseLab.Api/` | Controllers, API key filter, contracts |
| `backend/tests/` | Unit, API, integration (Testcontainers) and algorithm tests, with exercises. Add yours here. |
| `backend/benchmarks/` | BenchmarkDotNet |
| `desktop/` | Windows Forms app and FlaUI UI tests |
| `frontend/src/` | The React screen |
| `frontend/e2e/` | Playwright tests. Add yours here. |
| `perf/` | k6 scripts |
| `docker-compose.yml` | PostgreSQL for the app |
| `lessons/` | 17 lessons and the 7-day study plan |
| `exams/` | 3 exams |
| `tools/` | Install and first-use guides for every external tool |
| `CLAUDE.md`, `.claude/` | Context, skills, hooks, and a reviewer agent for AI coding agents |
| `ci/` | GitLab CI version of the pipelines, for study |
| `instructor/` | For the mentor only: hidden bugs and answer keys. Do not open. |

## If something fails

| What you see | What to do |
| --- | --- |
| `Now listening` never appears | Run `dotnet --version`. Install the .NET 10 SDK. |
| UI says it could not load taper plans | The API is not running, or not on port 5080. |
| UI shows old data or old screens | An old API or UI still runs. Stop it (Ctrl+C) and start again. |
| Port already in use | Close the old terminal or Rider run that still uses the port. |
| `npm` is not found | Install Node.js. Open a new terminal. |
| Playwright cannot find the browser | Run `npx playwright install chromium` in `frontend`. |
| k6 thresholds fail on a second run | Restart the API. Data from the last run is still in memory. |
| `dotnet test` fails with a Docker error | Start Docker Desktop, or run `dotnet test --filter "Category!=Docker"` |
