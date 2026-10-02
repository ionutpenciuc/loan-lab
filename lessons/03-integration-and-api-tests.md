# 03 — Integration and API tests

Time: 75 minutes.

Goal: test the real HTTP API in memory: status codes, headers, body, errors, and security, with a reusable test harness.

## Learn

### Kinds of integration test

| Kind | Example |
| --- | --- |
| Component integration | Service + real repository together |
| API integration | Send real HTTP requests through the whole ASP.NET pipeline: routing, filters, JSON |
| System integration | Your app + another real system (database, payment service) |
| Contract test | Check that the API shape is what the frontend expects. Tool: Pact |

### WebApplicationFactory

`WebApplicationFactory<Program>` starts the real API **in memory**. You get an `HttpClient`. There is no browser and no network port. It is fast (milliseconds per request) and real: routing, the API key filter, validation, and JSON all run.

```csharp
public class TaperPlansApiShould : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TaperPlansApiShould(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
}
```

`IClassFixture` starts **one** app for the whole class. All tests in the class share its data. So:

- Do not assert an exact number of plans. Another test may have added one.
- Use unique patient names.
- Or start a fresh app per test when you need a clean state.

### What to check in an API test

1. **Status code.** Right code for each case.
2. **Headers.** For example `Location` after a create.
3. **Body.** Field names and values (the contract with the frontend).
4. **Errors.** Message is clear. No stack trace leaks.
5. **Security.** No key → 401. Wrong key → 401.
6. **Side effects.** Preview must not save. A rejected save must not save.

### Status codes to know

| Code | Meaning | Dose lab |
| --- | --- | --- |
| 200 OK | Success with a body | GET list, preview |
| 201 Created | New resource. `Location` header points to it | POST create |
| 204 No Content | Success, no body | (not used) |
| 400 Bad Request | The input is wrong | Dose above the maximum |
| 401 Unauthorized | Missing or wrong credentials | No `X-Api-Key` |
| 403 Forbidden | Known user, but not allowed | (not used) |
| 404 Not Found | No such resource | GET unknown plan id |
| 409 Conflict | Clashes with current state | (not used) |
| 500 Internal Server Error | The server broke. Always a bug | — |

### The API

| Method and path | Key needed | Body |
| --- | --- | --- |
| `GET /api/medications` | no | — |
| `GET /api/taper-plans` | no | — |
| `GET /api/taper-plans/{id}` | no | — |
| `POST /api/taper-plans/preview` | no | `{ "medicationCode": "CLM", "startingDailyDoseMg": 10, "weekCount": 3 }` |
| `POST /api/taper-plans` | `X-Api-Key: lab-dev-key` | `{ "patientName": "Ana Pop", "medicationCode": "STR", "startingDailyDoseMg": 40, "weekCount": 4 }` |

Explore it by hand first:

- Swagger: http://localhost:5080/swagger
- Rider: open `backend/DoseLab.http` and click the green arrows.

### Test harness

A **test harness** is the code that sets up the system for tests: start it, configure it, give it data, clean it. A custom factory is a small harness:

```csharp
using DoseLab.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DoseLab.ApiTests;

public sealed class DoseLabApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(SimulatedLatency.None);      // no fake database delay
        });
    }
}
```

Use it with `IClassFixture<DoseLabApiFactory>`. In `ConfigureTestServices` you can also replace the clock with a fixed one.

## Do

Work in `backend/tests/DoseLab.ApiTests/TaperPlansApiShould.cs`. A helper for requests with a key:

```csharp
private static HttpRequestMessage CreateRequest(object body, string? apiKey)
{
    var request = new HttpRequestMessage(HttpMethod.Post, "/api/taper-plans")
    {
        Content = JsonContent.Create(body),
    };
    if (apiKey is not null)
        request.Headers.Add("X-Api-Key", apiKey);
    return request;
}
```

1. **Create returns 201.** POST a valid plan with the key. Assert `HttpStatusCode.Created` and that `response.Headers.Location` is not null. Then GET the `Location` URL. Assert it returns the same plan.
2. **No key → 401.** Then GET the list and assert the plan was **not** saved.
3. **Wrong key → 401.**
4. **Dose too high → 400** with the message `Starting daily dose must not exceed 20 mg for Nervalin.`
5. **Unknown id → 404.** Use `Guid.NewGuid()`.
6. **Preview saves nothing.** Count the plans before and after a preview.
7. **Harness.** Add `DoseLabApiFactory` from above and switch your class to it.

## Practice bug hunt 1

Ask your mentor to turn on one backend bug, or run `instructor/bug.sh random 1`. API tests start their own app, so no restart is needed. Run `dotnet test`. Did one of your tests fail? If yes, read the failure, and write a short bug report (see lesson 10). If not, it may be a UI or performance bug: run `instructor/bug.sh list`, then `instructor/bug.sh reset`.

## Check yourself

1. What does WebApplicationFactory run, and what does it not run?
2. Which status code for: create, missing key, unknown id, bad input?
3. Why should you not assert "the list has exactly 3 plans" in a shared fixture?
4. What is a test harness? Show the one you wrote.
5. Why test that a rejected save did not save anything?

## Say it in the interview

- "Most of my backend coverage is API-level with WebApplicationFactory: real pipeline, in memory, no flaky network."
- "I always check status, headers, body, error message, and side effects, and the negative security cases: no key, wrong key."
- "I build a small harness: a custom factory that sets config and swaps services, like the clock."
