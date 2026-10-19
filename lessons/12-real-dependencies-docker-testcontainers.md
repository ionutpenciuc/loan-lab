# 12 — Real dependencies: Docker, PostgreSQL and Testcontainers

Time: 90 minutes.

Goal: test against a real database that starts and stops with your tests, and understand when an in-memory fake is not enough.

## Learn

### Why fakes are not enough

The in-memory repository is fast and simple. But it cannot show:

| Real-world risk | Example |
| --- | --- |
| Data types and precision | A `numeric(6,2)` column silently rounds 12.345 to 12.35 |
| Text and encoding | Names with `'`, `ș`, `ț`, emoji, other alphabets |
| Queries | SQL syntax, SQL injection, sorting, missing index |
| Concurrency | Two saves at once; unique keys; transactions |
| Persistence | Data still there after a restart; no duplicate seed data |
| Migrations | The schema change on an old database |

So the pyramid has a layer between unit and UI: **integration tests against real dependencies**.

### Docker in one minute

- An **image** is a packaged app (for example `postgres:17-alpine`). A **container** is a running copy of an image.
- `docker compose up -d` starts the containers listed in `docker-compose.yml`. `docker compose down -v` stops them and deletes their data.
- Containers make test environments the same on every machine and in CI ("works on my machine" goes away).

### Testcontainers

A library that starts a fresh container **from your test code**, waits until it is ready, gives you a connection string, and removes it at the end.

```csharp
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public Task InitializeAsync() => _container.StartAsync();            // once, before the first test
    public Task DisposeAsync() => _container.DisposeAsync().AsTask();    // once, after the last test
    public async Task<string> CreateDatabaseAsync() { ... }              // a new empty database per test
}
```

- One container for all tests (`ICollectionFixture`): starting it takes a few seconds.
- One new database **per test**: tests never share data, so they never depend on order.
- Tests carry `[Trait("Category", "Docker")]`. On a laptop without Docker: `dotnet test --filter "Category!=Docker"`.

Testcontainers has modules for SQL Server, MongoDB, Redis, RabbitMQ, Kafka, Azure emulators, and any image you name.

### Three ways to test with a database

| Approach | Good | Bad |
| --- | --- | --- |
| In-memory fake | Fastest; no setup | Not the real behavior |
| Shared test database | Realistic | Tests clash; leftover data; order dependence |
| Testcontainers, one database per test | Realistic and isolated | Needs Docker; a few seconds to start |

### System tests

With `Lab:Storage = Postgres`, the **whole app** runs on the real database. `TaperPlansApiOnPostgresShould` starts the API twice on the same database to prove that data survives a restart. Combined with Playwright and k6 against this setup, you have system tests: all real parts working together.

## In this repo

| Path | What |
| --- | --- |
| `docker-compose.yml` | A PostgreSQL for running the API by hand (port 5433) |
| `backend/src/DoseLab.Postgres/PostgresTaperPlanRepository.cs` | The SQL. Read it: plain `SELECT` and `INSERT` with parameters. |
| `backend/src/DoseLab.Api/Program.cs` | The `Lab:Storage` switch |
| `backend/src/DoseLab.Api/Properties/launchSettings.json` | Launch profile `http-postgres` |
| `backend/tests/DoseLab.IntegrationTests/Harness/PostgresFixture.cs` | The Testcontainers fixture |
| `backend/tests/DoseLab.IntegrationTests/*.cs` | Samples and exercises L12-1 to L12-4 |

## Do

First install Docker Desktop: [tools/docker-and-postgres.md](../tools/docker-and-postgres.md). Check with `docker run --rm hello-world`.

1. **Run the API on PostgreSQL.** From the repo root: `docker compose up -d`. Then from `backend`:
   ```bash
   dotnet run --project src/DoseLab.Api --launch-profile http-postgres
   ```
   Start the UI. Save a plan. Stop the API (Ctrl+C) and start it again. Is your plan still there? Look at the table:
   ```bash
   docker compose exec db psql -U doselab -c "select * from taper_plans;"
   ```
2. **Run the integration tests.** From `backend`: `dotnet test --filter "Category=Docker"`. While they run, run `docker ps` in another terminal: you will see the test container. After: it is gone.
3. **L12-1: precision.** Complete `KeepTheStartingDoseExactly`. Which values did you choose, and why?
4. **L12-2: error guessing for names.** Complete `StoreAPatientNameExactlyAsWritten` with at least 6 names. Think like an attacker and like a nurse in Romania.
5. **L12-3: order.** Complete `ListPlansInTheOrderTheyWereSaved`.
6. **L12-4: seed data.** Complete `SeedTheSamplePlansOnlyOnce`.
7. **Find out what Docker hides.** Run `dotnet test --filter "Category=Docker"` with Docker Desktop **stopped**. Read the error. Write in `notes/integration.md` what a new team member would see, and what you would put in the README to help them.
8. **Bug hunt.** `instructor/bug.sh on 09`. Run all your tests. Which test fails? Which layer is this bug in, and which OWASP category? Then `instructor/bug.sh off 09`.

## Check yourself

1. Name three bugs an in-memory fake cannot find.
2. Image vs container?
3. What does Testcontainers do before and after your tests?
4. Why one database per test, and not one per run?
5. How do you run the unit tests on a machine without Docker?
6. How do CI pipelines get Docker for Testcontainers? (GitHub Linux runners have it; GitLab uses a `docker:dind` service: see `ci/gitlab-ci.example.yml`.)

## Say it in the interview

- "Fakes are great for rules. For the data layer I use Testcontainers: a real PostgreSQL per test run, a fresh database per test, gone at the end. Same in CI."
- "My error-guessing list for any text field includes quotes, local letters, emoji, other alphabets, SQL and HTML. In my lab, an apostrophe in 'O'Brien' exposed a SQL injection."
- "I test persistence explicitly: save, restart, read back, and no duplicated seed data."
