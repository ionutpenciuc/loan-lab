---
name: write-unit-test
description: Writes xUnit unit tests for Dose lab application code (taper calculator, taper plan service). Use when asked to add, extend, or fix a C# unit test in backend/tests/DoseLab.UnitTests.
---

# Write a unit test

1. Find the rule. Read `CLAUDE.md` (Domain rules) and the class under test in `backend/src/DoseLab.Application/`. Write the rule in one sentence before you write code.
2. Choose the cases with test-design techniques:
   - Boundary values: on and next to each limit, at the input precision (0.01 mg): for 1–20 use 0.99, 1, 20, 20.01.
   - Equivalence partitions: one value per valid and invalid group.
   - Error guessing: empty, spaces, `null`, very long, uneven division.
3. Work out the expected values **by hand** from the rule. Never copy them from running the code.
4. Write the test in the right file:
   - Calculator → `TaperScheduleCalculatorShould.cs`
   - Service → `TaperPlanServiceShould.cs`
   Class `{Subject}Should`. Method name finishes the sentence. Use `[Theory]` + `[InlineData]` for several inputs of the same rule.
5. Body layout, each tag followed by one empty line:
   ```csharp
   // setup

   var service = new TaperPlanService(
       new InMemoryTaperPlanRepository(SimulatedLatency.None),
       new InMemoryMedicationCatalog(SimulatedLatency.None),
       TimeProvider.System);

   // execute

   var error = Record.Exception(() => service.Preview("NRV", 20m, 4));

   // verify

   Assert.Null(error);
   ```
   Use a fixed `TimeProvider` when the test checks a date.
6. Run `dotnet test` in `backend`. The new test must pass.
7. Prove it can fail: change one expected value, run, see it fail, put it back.
8. If a test fails and the expected value is right by the rule, do **not** change product code. Report it as a possible bug with the triage-test-failure skill.

## Done when

- Every case links to a rule or a boundary.
- All tests pass, and each new test was seen failing once.
- No product code changed.
