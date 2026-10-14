---
name: find-flaky-tests
description: Finds flaky tests in Dose lab by running a test suite several times and comparing results. Use when a test passes and fails on the same code, or before moving tests into the blocking CI path.
---

# Find flaky tests

<!--
Lesson 16, exercise TODO(L16-2). This skill is a stub. Finish it, then try it on
backend/tests/DoseLab.ApiTests/FlakyExamplesShould.cs (with the Skip removed).
Keep it short: an agent follows short, exact steps better than long text.
-->

## Steps

1. Ask which suite to check, or use the one named in the request:
   - .NET: `dotnet test <project> --logger "junit;LogFileName=run-{n}.xml"` from `backend`
   - Playwright: `npx playwright test --repeat-each=5` from `frontend`
2. TODO(L16-2): run the suite N times. How many? Where do results go? How does the agent compare them?
3. TODO(L16-2): also run each suspect test **alone** and **with its class**. Why both?
4. TODO(L16-2): for each test that changed result, find a likely cause from the list in lessons/11 (shared data, order, time, sleeps, races, locators, environment).

## Output

TODO(L16-2): a table: test name, passes / runs, alone vs together, likely cause, suggested fix.
Then one line: `Quarantine: <tests>` or `Quarantine: none`.

## Rules

- Never change product code. Never delete a test.
- Do not mark a test flaky after one run. Show the numbers.
