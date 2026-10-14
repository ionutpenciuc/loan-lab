# 11 — Test harness engineering in .NET

Time: 2 hours.

Goal: build and evolve test harnesses that are fast, isolated and easy to read: mocks, fake clocks, data builders, custom fixtures, categories, flaky test control, coverage and mutation testing.

The job ad: "build and evolve test harnesses ... that catch regressions early, run reliably at scale, and give fast, trustworthy feedback". Lessons 02 and 03 taught you to write tests. This lesson is about the **machinery around tests**: the part a senior owns.

## Learn: what a test harness is

Everything that starts, configures, feeds and observes the system under test:

| Part | .NET tool | In this repo |
| --- | --- | --- |
| Test runner and framework | xUnit (also NUnit, MSTest) | every test project |
| Assertions | xUnit `Assert`, FluentAssertions, Shouldly | `TaperPlanServiceWithMocksShould.cs` |
| Test doubles | NSubstitute, Moq, FakeItEasy, hand-written fakes | NSubstitute, `InMemory*` fakes |
| Time control | `TimeProvider` + `FakeTimeProvider` | `Microsoft.Extensions.TimeProvider.Testing` |
| Test data | Builders, object mothers, Bogus (fake names) | `Builders/TaperPlanBuilder.cs` |
| Host / app under test | `WebApplicationFactory<Program>` | `ApiTests/Harness/DoseLabApiFactory.cs` |
| Real dependencies | Testcontainers | lesson 12 |
| Shared setup | `IClassFixture`, `ICollectionFixture`, `IAsyncLifetime` | factory, `PostgresFixture` |
| Selection | `[Trait]` + `--filter` | `Category=Docker`, `Category=DesktopUi` |
| Reports | trx, JUnit XML, coverage (Cobertura) | `.github/workflows/ci.yml`, `ci/gitlab-ci.example.yml` |
| Test quality | Coverage, mutation testing (Stryker.NET) | `backend/tests/DoseLab.UnitTests/stryker-config.json` |

## Learn: mocks with NSubstitute

```csharp
var catalog = Substitute.For<IMedicationCatalog>();               // a test double for an interface
catalog.Find("CLM").Returns(new Medication("CLM", "Calmafen", 40m));   // stub: give an answer
catalog.Find("XXX").Returns((Medication?)null);

repository.Received(1).Add(Arg.Is<NewTaperPlan>(d => d.PatientName == "Ana Pop"));   // mock: check a call
repository.DidNotReceive().Add(Arg.Any<NewTaperPlan>());
```

Moq does the same with a different style: `var mock = new Mock<IMedicationCatalog>(); mock.Setup(c => c.Find("CLM")).Returns(...); mock.Verify(...)`. Know that both exist. Many companies use Moq.

**When to mock, and when not to.**

- Mock what you do not own or what is slow: email, payment, a remote service, a clock.
- Prefer real objects or fakes for simple, fast code. `InMemoryTaperPlanRepository` is often better than a mock.
- Verify a call only when the call **is** the rule: "nothing is saved when validation fails", "the catalog is read once, not per plan". Do not mirror every line of the implementation. Such tests break on every refactor and find no bugs (over-specification).

## Learn: time, data, and readable asserts

- **Time.** Code that reads `DateTime.Now` cannot be tested at midnight, on 29 February, or across time zones. This app takes a `TimeProvider`. Tests pass a `FakeTimeProvider` and move it: `clock.SetUtcNow(...)`, `clock.Advance(TimeSpan.FromDays(1))`.
- **Test data builders.** A test should show only the values it cares about. `TaperPlanBuilder.APlan().WithMedication("CLM").BuildMany(200)` makes 200 valid plans in one line. When the model gets a new required field, you change one builder, not 50 tests.
- **FluentAssertions.** `plans.Should().ContainSingle(p => p.PatientName == "Ana Pop")` reads like a sentence and gives clearer failure messages. **Licence note:** FluentAssertions 8 needs a paid licence for commercial use. This repo pins 7.2.2 (Apache 2.0, free). Free alternatives: Shouldly, AwesomeAssertions (a fork of 7). Knowing this is a good senior signal: you check licences of test tools too.

## Learn: fixtures and the custom factory

xUnit creates a **new test class object for every test**. Use that for isolation. For expensive setup, share it:

| Need | xUnit | Lifetime |
| --- | --- | --- |
| Setup per test | constructor / `IDisposable` / `IAsyncLifetime` | one test |
| Share inside one class | `IClassFixture<T>` | all tests of the class |
| Share across classes | `[Collection("Name")]` + `ICollectionFixture<T>` | all classes in the collection (they do not run in parallel with each other) |

`DoseLabApiFactory` starts the real API in memory and changes three things for tests: no simulated delay, a fake clock, and its own API key. Read it. This pattern (subclass `WebApplicationFactory`, override `ConfigureWebHost`, use `UseSetting` and `ConfigureTestServices`) is how most .NET teams build API test harnesses.

## Learn: flaky tests

A flaky test passes and fails on the same code. It destroys trust: people start to ignore red builds.

1. **Detect.** Re-run failures; track pass rate per test over time (CI history, a flaky report).
2. **Quarantine.** Move it out of the blocking path at once: xUnit `[Fact(Skip = "Quarantined: <ticket>")]` or `[Trait("Category", "Quarantine")]` + `--filter "Category!=Quarantine"`; Playwright `test.fixme()` or a `@quarantine` tag with `--grep-invert`.
3. **Fix the cause.** Shared data, test order, time, fixed sleeps, race conditions, weak locators, environment.
4. **Return it** to the blocking suite. Retries hide flakiness. Use them only with reporting (Playwright marks a test that passed on retry as "flaky").

## Learn: coverage and mutation testing

**Coverage** says which lines ran. It does not say any result was checked.

```bash
# from the repo root
dotnet test backend/DoseLab.slnx --collect "XPlat Code Coverage" --results-directory TestResults
cd backend && dotnet tool restore
dotnet reportgenerator -reports:"../TestResults/**/coverage.cobertura.xml" -targetdir:../coverage-report -reporttypes:Html
# open coverage-report/index.html
```

**Mutation testing** checks the tests. Stryker.NET changes the code in small ways ("mutants": `>` to `>=`, `+` to `-`, remove a line), runs your tests for each one, and reports which mutants **survived** (no test failed). A survived mutant is a missing or weak test. The hidden bugs in this lab are hand-made mutants.

```bash
cd backend/tests/DoseLab.UnitTests
dotnet tool restore
dotnet stryker            # about 1 minute; opens nothing, prints the report path
```

Open `StrykerOutput/<date>/reports/mutation-report.html`. Mutation score = killed ÷ all. Today it is about 50%. Install guide: [tools/stryker-and-coverage.md](../tools/stryker-and-coverage.md).

## In this repo

| File | What to look at |
| --- | --- |
| `backend/tests/DoseLab.UnitTests/TaperPlanServiceWithMocksShould.cs` | NSubstitute, FakeTimeProvider, FluentAssertions; exercises L11-1 to L11-4 |
| `backend/tests/DoseLab.UnitTests/Builders/TaperPlanBuilder.cs` | Test data builder |
| `backend/tests/DoseLab.ApiTests/Harness/DoseLabApiFactory.cs` | Custom `WebApplicationFactory` |
| `backend/tests/DoseLab.ApiTests/TaperPlansApiWithCustomFactoryShould.cs` | Using it; exercise L11-5 |
| `backend/tests/DoseLab.ApiTests/FlakyExamplesShould.cs` | A quarantined flaky test; exercise L11-6 |
| `backend/tests/DoseLab.UnitTests/stryker-config.json` | Stryker settings |

## Do

Find all exercises: `git grep "TODO(L11"`.

1. **L11-1: catch a performance bug with a mock.** Complete `ReadTheMedicationCatalogOnceWhenListingPlans`. It must fail first (remove Skip, run), then pass. Then run `instructor/bug.sh on 07`, run it again: it must fail. `instructor/bug.sh off 07`. Bug 07 is a performance bug, and you just caught it in 50 ms at unit level. Write down the trade-off: is this test too close to the implementation?
2. **L11-2.** No call reaches the repository during a preview.
3. **L11-3: time.** Move the fake clock. Then answer the question in the TODO about a nurse in Bucharest. Write it in `notes/questions.md`. Asking this is a senior move: the code is "correct", but is the rule right?
4. **L11-4.** Rewrite one assert with FluentAssertions. Break it on purpose. Compare the two failure messages.
5. **L11-5: harness configuration.** Complete the API key test. Explain why the factory uses its own key.
6. **L11-6: flaky test.** Follow the steps in `FlakyExamplesShould.cs`. Write the root cause in one sentence. Fix the test, not the app.
7. **Categories.** Add `[Trait("Category", "Smoke")]` to 3 fast, important tests (unit or API). Run only them: `dotnet test --filter "Category=Smoke"`. This is how you build a 30-second smoke suite for every push.
8. **Coverage.** Generate the HTML report. Find one line in `TaperPlanService.cs` that no test runs. Is it important? Write a test for it if yes.
9. **Mutation testing.** Run Stryker. Pick 3 survived mutants in `TaperPlanService.cs` or `TaperScheduleCalculator.cs`. For each: what change did Stryker make, and which test would kill it? Write those tests. Run Stryker again. Did the score go up? Note the before and after scores in `notes/mutation.md`.

## Check yourself

1. Stub vs mock: what is the difference? Give one example of each from this repo.
2. When is mocking a bad idea?
3. Why does this app take a `TimeProvider` instead of calling `DateTime.UtcNow`?
4. What problem does a test data builder solve?
5. `IClassFixture` vs `ICollectionFixture`: when do you use each?
6. Name four causes of flaky tests. What do you do in the first hour after you find one?
7. 90% line coverage, 40% mutation score. What does that tell you?
8. Why can a mock catch a performance bug, and what is the risk of that test?

## Say it in the interview

- "A test harness is a product. I design it: a custom WebApplicationFactory that swaps the clock and settings, builders for test data, fixtures for expensive setup, and traits so CI can pick smoke, Docker or UI suites."
- "I mock only at boundaries, and I verify a call only when the call is the requirement, like 'nothing is saved when validation fails'."
- "Coverage tells me what ran; mutation testing with Stryker.NET tells me what was actually checked. I use the survived mutants as a to-do list."
- "Flaky tests go to quarantine the same day, with a ticket. Then I fix the cause: usually shared data, order, or time."
