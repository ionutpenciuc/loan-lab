# 02 — C# for testers and unit tests

Time: 90 minutes.

Goal: read the C# in this repo, and write good xUnit tests with test-design techniques.

## Learn: just enough C#

```csharp
namespace DoseLab.Application;                 // a folder-like name for code

public sealed record Medication(               // a record: a small data type, compared by value
    string Code, string Name, decimal MaxDailyDoseMg);

public static class TaperScheduleCalculator    // a class; "static" = no object needed
{
    public static TaperSchedule Calculate(decimal startingDailyDoseMg, int weekCount)
    {
        if (weekCount < 1)
            throw new ArgumentOutOfRangeException(nameof(weekCount));   // an exception = an error
        var step = Math.Floor(startingDailyDoseMg / weekCount * 100m) / 100m;  // var = type is inferred
        ...
    }
}
```

What to know:

| Item | Meaning |
| --- | --- |
| `decimal` | Exact decimal numbers. Use for money and doses. `10m` is a decimal literal. |
| `double` | Fast, but binary: `0.1 + 0.2` is `0.30000000000000004`. Do not use for doses. |
| `string?` | The `?` means the value can be `null`. |
| `x => x.Code` | A lambda: a small inline function. |
| `.Select(...)`, `.Where(...)`, `.FirstOrDefault(...)` | LINQ: work on lists. Like `map`, `filter`, `find` in JavaScript. |
| `throw new TaperValidationException("...")` | Stop and report an error. Tests check for it with `Assert.Throws`. |
| Interface (`IMedicationCatalog`) | A contract. Tests can pass a fake that follows the same contract. |
| Constructor injection | A class gets its helpers in the constructor: `new TaperPlanService(repository, catalog, clock)`. This makes testing easy. |

## Learn: xUnit

```csharp
public class TaperScheduleCalculatorShould          // {Subject}Should
{
    [Fact]                                           // one test, no inputs
    public void LowerTheDailyDoseByTheSameStepEveryWeek()   // the name finishes the sentence
    {
        // setup

        var startingDailyDoseMg = 40m;

        // execute

        var schedule = TaperScheduleCalculator.Calculate(startingDailyDoseMg, 4);

        // verify

        Assert.Equal(30m, schedule.Weeks[1].DailyDoseMg);
    }

    [Theory]                                         // one test, many inputs
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void ApplyTheWeekLimits(int weekCount, bool accepted) { ... }
}
```

Useful asserts: `Assert.Equal`, `Assert.True`, `Assert.Single`, `Assert.Empty`, `Assert.Contains`, `Assert.Throws<T>(() => ...)`, `Record.Exception(() => ...)`.

`// setup` / `// execute` / `// verify` is the same idea as **Arrange / Act / Assert** (AAA) or **Given / When / Then**.

A good unit test is **FIRST**: Fast, Isolated (no shared state), Repeatable (same result every run), Self-validating (pass or fail, no reading logs), Timely (written with the code).

## Learn: test-design techniques (ISTQB black-box)

| Technique | Idea | Dose lab example |
| --- | --- | --- |
| Equivalence partitioning | Split inputs into groups that behave the same. Test one per group. | Dose for Nervalin: below 1 (invalid), 1–20 (valid), above 20 (invalid) |
| Boundary value analysis | Bugs hide at edges. Test on and next to each edge. | 0.99, 1, 20, 20.01 |
| Decision table | Combinations of conditions → result | Medication known? × dose in range? × weeks in range? |
| State transition | Allowed moves between states | (Not in this app. Example: draft → active → stopped.) |
| Error guessing | Use experience: empty, spaces, null, very long, special characters | Patient name `"   "` |

## Learn: test doubles

| Double | What it is | In this repo |
| --- | --- | --- |
| Fake | A simple working version | `InMemoryTaperPlanRepository` (a list instead of a database) |
| Stub | Returns fixed answers | A fixed clock: `FixedClock : TimeProvider` |
| Mock | Checks that a call happened | Not used here. Libraries: Moq, NSubstitute |
| Spy | Records calls for later checks | Not used here |

`SimulatedLatency.None` turns off the fake database delay in unit tests.

**Never depend on today's date** in a test. Pass a fixed clock (`TimeProvider`) instead.

## Learn: is my test any good?

A green test proves nothing until you have seen it fail for the right reason.

- **Red/green check.** Break the expected value. The test must fail. Put it back.
- **Mutation testing.** A tool (Stryker.NET) makes small changes to the code ("mutants"). Good tests fail ("kill the mutant"). The hidden bugs in this lab are hand-made mutants.
- **Coverage** shows which lines ran. It does not show that you checked the result. Use it to find untested code, not as a target.

## Do

All work goes in `backend/tests/DoseLab.UnitTests/`. Run `dotnet test` from `backend` after each test. In Rider, you can also click the green arrow next to a test.

### 1. The rounding remainder

The rule: step = starting dose ÷ weeks, rounded **down** to 0.01 mg. Week *n* dose = start − (n − 1) × step.

Calculate by hand for 10 mg over 3 weeks: the step, each week's daily dose, each weekly total (× 7), and the total. Then write `KeepTheRoundingRemainderInTheLastWeek` in `TaperScheduleCalculatorShould.cs`.

Why this case matters: 40 mg / 4 weeks divides evenly. 10 / 3 does not. Edge cases hide bugs.

### 2. Boundaries for the dose limit

In `TaperPlanServiceShould.cs`, write a `[Theory]` for Nervalin (`NRV`, max 20 mg, min 1 mg): 0.99, 1, 20, 20.01. Use `service.Preview(...)` and `Record.Exception`.

Then: Steriva (`STR`) at exactly 80 mg must be **accepted**. Write it as its own `[Fact]`, with a clear name.

### 3. Weeks: 0, 1, 52, 53

Another `[Theory]`.

### 4. Bad patient names

`""`, `"   "`, and `null` must give `"Patient name is required."`. Also: a 101-character name is rejected (`new string('a', 101)`).

### 5. Preview saves nothing

Call `Preview`, then assert `service.List()` is empty.

### 6. A fixed clock

Write a small class:

```csharp
private sealed class FixedClock : TimeProvider
{
    private readonly DateTimeOffset _now;
    public FixedClock(DateTimeOffset now) { _now = now; }
    public override DateTimeOffset GetUtcNow() => _now;
}
```

Pass it to the service. Assert `CreatedOn` is the fixed date.

### 7. Red/green

Change one expected value in a test you wrote. Run. See the failure message. Put it back.

## Check yourself

1. Why `decimal` and not `double` for doses?
2. What is the difference between `[Fact]` and `[Theory]`?
3. Which four values do you test for the range 1–20 with boundary value analysis?
4. What is a fake? Give an example from this repo.
5. Why must a test never use the real current date?
6. A test is green. How do you know it can fail?

## Say it in the interview

- "I use boundary value analysis on every limit. For a 1–20 mg range I test 0.99, 1, 20, and 20.01."
- "I make sure each test can fail: I break the expectation once, or I use mutation testing with Stryker.NET."
- "I keep unit tests isolated: fakes for the database, a fixed clock for dates."
