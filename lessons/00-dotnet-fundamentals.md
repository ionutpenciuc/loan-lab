# 00 — .NET and C# fundamentals for testers

Time: 2 hours.

Goal: find your way around a .NET solution, use the `dotnet` command line, read the C# in this repo, and debug a test. Everything later builds on this.

The job ad asks for "strong programming skills", with C# as a plus. A senior tester does not need to design the app. You need to read it, change test code with confidence, and build test projects from scratch.

## Learn: the pieces

| Word | What it is | In this repo |
| --- | --- | --- |
| .NET SDK | The tools to build and run code: the `dotnet` command, the compiler. Install the SDK, not only the runtime. | `dotnet --version` must start with `10.` |
| Runtime | Runs a built app. The SDK includes it. | — |
| Project (`.csproj`) | One unit that builds into one `.dll` or `.exe`. Lists its packages and the projects it uses. | `backend/src/DoseLab.Api/DoseLab.Api.csproj` |
| Solution (`.slnx` / `.sln`) | A list of projects that belong together. IDEs open it. `.slnx` is the new, short XML form. | `backend/DoseLab.slnx`, `desktop/DoseLab.Desktop.slnx` |
| `Directory.Build.props` | Settings shared by every project in the folder below it | `backend/Directory.Build.props` (target framework, nullable) |
| Target framework | Which .NET the project builds for. `net10.0` runs anywhere; `net10.0-windows` needs Windows. | `net10.0`, desktop: `net10.0-windows` |
| NuGet package | A library from nuget.org. Like npm for .NET. | `xunit`, `Npgsql`, `FluentAssertions` |
| Project reference | One project uses another project's code | Tests reference `DoseLab.Application` |
| Assembly | The built `.dll`. Test runners load it. | `bin/Debug/net10.0/DoseLab.UnitTests.dll` |
| Namespace | A name to group code: `DoseLab.Application`. Matches the folder by habit. | `namespace DoseLab.UnitTests;` |

### The solution here

```text
backend/
  src/
    DoseLab.Application   rules: calculator, service, interfaces, in-memory fakes       (no web, no database)
    DoseLab.Postgres      the PostgreSQL repository                                     (lesson 12)
    DoseLab.Api           ASP.NET Core Web API: controllers, Program.cs, settings
    DoseLab.Algorithms    interview algorithms
  tests/
    DoseLab.UnitTests         xUnit, no I/O
    DoseLab.ApiTests          the API in memory (WebApplicationFactory)
    DoseLab.IntegrationTests  real PostgreSQL in Docker (Testcontainers)
    DoseLab.AlgorithmTests
  benchmarks/
    DoseLab.Benchmarks        BenchmarkDotNet                                           (lesson 13)
desktop/                       Windows Forms app + FlaUI UI tests                       (lesson 15)
```

Arrows go one way: `Api → Application ← Postgres`. The rules do not know about HTTP or databases. This is why they are easy to unit test.

## Learn: the `dotnet` command

Run these from `backend`:

| Command | What it does |
| --- | --- |
| `dotnet build` | Restores packages and compiles everything in the solution |
| `dotnet test` | Builds and runs every test project |
| `dotnet test --filter "FullyQualifiedName~TaperPlanService"` | Only tests whose full name contains the text |
| `dotnet test --filter "Category!=Docker"` | Skip tests marked `[Trait("Category", "Docker")]` |
| `dotnet test --logger "console;verbosity=detailed"` | Show each test and its output |
| `dotnet run --project src/DoseLab.Api` | Start the API |
| `dotnet new xunit -o tests/My.Tests` | Create a new xUnit test project |
| `dotnet sln DoseLab.slnx add tests/My.Tests/My.Tests.csproj` | Add it to the solution |
| `dotnet add tests/My.Tests reference src/DoseLab.Application` | Let it use the app code |
| `dotnet add tests/My.Tests package FluentAssertions --version 7.2.2` | Add a NuGet package |
| `dotnet list package --outdated` / `--vulnerable` | Old or unsafe packages |
| `dotnet tool restore` | Install the tools listed in `backend/dotnet-tools.json` (Stryker, ReportGenerator) |
| `dotnet clean` | Delete build output. Try it when the build acts strange. |

## Learn: C# you will read every day

```csharp
public sealed record Medication(string Code, string Name, decimal MaxDailyDoseMg);   // data, compared by value

public interface ITaperPlanRepository                     // a contract; tests can pass a fake or a mock
{
    IReadOnlyList<TaperPlan> List();
    TaperPlan Add(NewTaperPlan draft);
}

public sealed class TaperPlanService(ITaperPlanRepository repository, IMedicationCatalog catalog, TimeProvider clock)
{                                                          // ↑ "primary constructor": the helpers it needs
    public TaperPlanSummary? Find(Guid id)                 // ? = may return null
    {
        var plan = repository.List().FirstOrDefault(candidate => candidate.Id == id);   // LINQ + lambda
        if (plan is null)
            return null;

        var medication = catalog.Find(plan.MedicationCode);
        return new TaperPlanSummary(plan, medication?.Name ?? plan.MedicationCode);       // ?. and ??
    }
}
```

| Feature | Meaning | Why a tester cares |
| --- | --- | --- |
| `class` / `record` / `struct` | Reference type / data type compared by value / small value type | `Assert.Equal` on records compares all fields |
| `var` | The compiler works out the type | Hover in the IDE to see it |
| `string?`, `?.`, `??` | Nullable reference types: "can be null", "call only if not null", "use this if null" | Null is a classic bug source. Test it. |
| Properties `{ get; set; }` | Fields with access rules | Request contracts in `Contracts/` |
| Interfaces | A contract with no code | The seam where tests swap real parts for test doubles |
| Generics `List<T>` | A type that works for any `T` | `IReadOnlyList<TaperPlan>` |
| LINQ | `Where`, `Select`, `First(OrDefault)`, `Any`, `All`, `Count`, `Sum`, `OrderBy`, `GroupBy`, `ToDictionary` | Most test setup and asserts use it |
| Lambda `x => x.Code` | A small inline function | Used by LINQ, mocks, asserts |
| Exceptions `throw` / `try` / `catch` | Errors that stop the code | `Assert.Throws<T>`; an uncaught one in the API is a 500 |
| `async` / `await` / `Task` | Code that waits (HTTP, database) without blocking a thread | API tests are `async Task`. Never use `.Result` or `.Wait()` in tests: it can deadlock. |
| Attributes `[Fact]`, `[HttpGet]` | Labels the framework reads | xUnit finds tests by them |
| `decimal` vs `double` | Exact base-10 vs fast binary | Doses use `decimal`. `0.1 + 0.2 != 0.3` in `double`. |
| `string.Format` / culture | `10.5` prints as `10,5` on a Romanian Windows | A whole class of bugs on desktop and in reports (lesson 15) |

## Learn: how the API starts

Open `backend/src/DoseLab.Api/Program.cs`. Read it top to bottom. It does three things:

1. **Registers services** in the dependency injection (DI) container: `builder.Services.AddSingleton<ITaperPlanRepository, InMemoryTaperPlanRepository>()`. When a controller needs an `ITaperPlanRepository`, the container gives it this one.
   - `AddSingleton`: one object for the whole app (the in-memory data lives as long as the app).
   - `AddScoped`: one per HTTP request (typical for a database context).
   - `AddTransient`: a new one every time.
2. **Reads configuration**: `appsettings.json`, then `appsettings.Development.json`, then environment variables, then command-line arguments. Later wins. `Lab:ApiKey` in JSON is `Lab__ApiKey` as an environment variable (two underscores).
3. **Builds the middleware pipeline**: each HTTP request passes through Swagger, CORS, then routing to a controller. A filter (`[RequireApiKey]`) runs before the controller action.

Why testers care: `WebApplicationFactory` (lesson 03, 11) starts this same `Program`, and lets a test **replace** a registered service or a setting. That is the most useful trick in .NET test automation.

## Learn: debugging a test

In Visual Studio or Rider: click in the left margin of a line to set a **breakpoint** (red dot). Right-click the test → **Debug**. When it stops: hover over variables, use **Step Over** (F10) and **Step Into** (F11), and the **Watch** window.

In VS Code: install the C# Dev Kit extension, then use the Testing panel → Debug Test.

## In this repo

- `backend/tests/DoseLab.UnitTests/Basics/CSharpBasicsShould.cs` — 8 short C# exercises, all skipped.
- `backend/src/DoseLab.Api/Program.cs` — DI, configuration, middleware.
- `backend/DoseLab.http` — ready-made HTTP requests.

## Do

1. **Build and test from the command line.** From `backend`: `dotnet build`, then `dotnet test`. If you have no Docker yet, run `dotnet test --filter "Category!=Docker"`. Note how many tests pass and how many are skipped.
2. **C# exercises.** Open `CSharpBasicsShould.cs`. For each test: remove `Skip = "..."`, run it, see it fail (red). Write the missing line under `// execute` until it passes (green). Do not change `// verify`. Run only this class with:
   ```bash
   dotnet test --filter "FullyQualifiedName~CSharpBasicsShould"
   ```
3. **Debug.** Set a breakpoint on the line `var step = ...` in `backend/src/DoseLab.Application/TaperScheduleCalculator.cs`. Debug the test `LowerTheDailyDoseByTheSameStepEveryWeek`. Write down the value of `step`. Step through the loop once.
4. **Create a test project from zero.** This is what you do when you build a new test harness:
   ```bash
   dotnet new xunit -o scratch/Playground
   dotnet add scratch/Playground reference src/DoseLab.Application
   dotnet test scratch/Playground
   ```
   Write one test in it that calls `TaperScheduleCalculator.Calculate(20m, 4)`. Then delete the `scratch` folder. (We do not add it to the solution.)
5. **Configuration.** Start the API with a different simulated delay, without changing any file:
   ```bash
   # PowerShell
   $env:Lab__SimulatedLatencyMs = "50"; dotnet run --project src/DoseLab.Api
   # Git Bash
   Lab__SimulatedLatencyMs=50 dotnet run --project src/DoseLab.Api
   ```
   Open http://localhost:5080/api/taper-plans. Is it slower? Close the terminal to reset.
6. **Read DI.** In `Program.cs`, find which class is used for `ITaperPlanRepository` and why it must be a singleton. What would happen to saved plans if it were `AddTransient`? Write the answer in `notes/dotnet.md`.

## Check yourself

1. What is the difference between a solution and a project?
2. What does `dotnet test --filter "Category!=Docker"` do?
3. `Lab:ApiKey` in `appsettings.json`: how do you set it with an environment variable?
4. Singleton vs scoped vs transient: one sentence each.
5. Why do tests in this repo use `decimal` for doses?
6. What does `?.` do? What does `??` do?
7. Why should a test never call `.Result` on a `Task`?

## Say it in the interview

- "I'm comfortable in C#: I build test projects from the CLI, add packages and references, and debug tests in the IDE."
- "In ASP.NET Core I use the DI container as a test seam: `WebApplicationFactory` lets me swap the clock, the database, or a setting for a test."
- "I watch for .NET-specific risks: null handling, `decimal` vs `double` for exact values, culture-dependent number formats, and blocking on async code."
