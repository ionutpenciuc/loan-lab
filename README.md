# Dose lab

A small medical-style app for learning test automation: unit, API, UI (Playwright), performance and stress (k6), CI, security, and AI-assisted testing.

Nurses create **taper plans**: a patient's daily dose of a medication goes down by the same step every week. The app shows the plans, previews the week-by-week schedule, and saves new plans.

The medications and their limits are **fictional**. This is not medical guidance.

- Backend: .NET 10 Web API, in `backend/`. Data is kept in memory. Restart the API and saved plans are gone. Two sample plans come back.
- Frontend: React + TypeScript (Vite), in `frontend/`.

**Start here after setup: [lessons/README.md](lessons/README.md)**, the 3-day study plan.

## Install the tools

| Tool | Check | Download |
| --- | --- | --- |
| Git | `git --version` | https://git-scm.com |
| .NET 10 SDK | `dotnet --version` (must start with `10.`) | https://dotnet.microsoft.com/download/dotnet/10.0 |
| Node.js 20 or newer | `node --version` | https://nodejs.org |
| k6 (for lesson 05) | `k6 version` | https://grafana.com/docs/k6/latest/set-up/install-k6/ |

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

In Rider: open `backend/DoseLab.slnx`, choose the `DoseLab.Api: http` run configuration, and press Run.

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

| Level | Command | Folder | Sample tests |
| --- | --- | --- | --- |
| Unit + API | `dotnet test` | `backend` | 3 taper samples, plus the algorithm tests |
| UI (Playwright) | `npx playwright install chromium` (once), then `npm run test:e2e` | `frontend` | 1 |
| Performance smoke | `k6 run perf/smoke.js` (API must be running) | repo root | 1 |

Playwright starts the API and the UI itself, or reuses them if they already run. To use other ports: `LAB_API_PORT=5181 LAB_UI_PORT=5182 npm run test:e2e`.

GitHub Actions runs all three levels on every push: `.github/workflows/ci.yml`.

## Repo map

| Path | What |
| --- | --- |
| `backend/src/DoseLab.Application/` | Rules: taper calculator, service, in-memory repository, medication catalog |
| `backend/src/DoseLab.Algorithms/` | Interview algorithms: search, sort, hash map, stack, graph, dynamic programming |
| `backend/src/DoseLab.Api/` | Controllers, API key filter, contracts |
| `backend/tests/` | Sample unit and API tests. Add yours here. |
| `frontend/src/` | The React screen |
| `frontend/e2e/` | Playwright tests. Add yours here. |
| `perf/` | k6 scripts |
| `lessons/` | 10 lessons and the study plan |
| `exams/` | 3 exams |
| `CLAUDE.md`, `.claude/` | Context, skills, and a reviewer agent for AI coding agents |
| `ci/` | GitLab CI version of the pipeline, for study |
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
