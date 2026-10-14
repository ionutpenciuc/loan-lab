---
name: find-flaky-tests
description: Finds flaky tests in Dose lab by running a test suite several times and comparing results. Use when a test passes and fails on the same code, or before moving tests into the blocking CI path.
---

# Find flaky tests

Answer key for L16-2. Any version that runs repeatedly, compares alone vs together, and reports numbers is fine.

## Steps

1. Pick the suite from the request. If none is named, ask.
2. Run it 5 times and keep each result:
   - .NET, from `backend`: `dotnet test <project> --results-directory ../TestResults/flaky --logger "junit;LogFileName=run-<n>.xml"`
   - Playwright, from `frontend`: `npx playwright test --repeat-each=5 --reporter=json > ../TestResults/flaky/playwright.json`
3. Compare: for each test, count passes / runs. Any test with 0 < passes < runs is a suspect.
4. Also run a test that always fails, or a suspect, **alone** (`--filter "FullyQualifiedName~<name>"`, or `-g "<name>"`) and **with its class or file**. Passing alone but failing together means order dependence or shared data, even if it is "stable" in one mode.
5. For each suspect, read the test and name the most likely cause: shared data, order, time or date, fixed sleeps, race condition, weak locator, environment.

## Output

| Test | Passes / runs | Alone | Together | Likely cause | Suggested fix |
| --- | --- | --- | --- | --- | --- |

Then one line: `Quarantine: <tests>` or `Quarantine: none`.

## Rules

- Never change product code. Never delete a test. Do not add retries as a fix.
- Do not mark a test flaky after one run. Show the numbers.
