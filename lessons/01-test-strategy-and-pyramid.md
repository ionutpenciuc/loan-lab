# 01 — Test strategy and the testing pyramid

Time: 45 minutes.

Goal: explain where each kind of test belongs, and write a one-page test strategy.

## Learn

### Test levels

| Level | What it tests | Speed | Example in Dose lab |
| --- | --- | --- | --- |
| Unit (component) | One class or function. No network, no database, no browser. | Milliseconds | The taper calculator |
| Integration | Parts working together: service + repository, or the full HTTP API in memory | Fast (sub-second) | `POST /api/taper-plans` returns 201 |
| System / end-to-end (E2E) | The whole app, as a user sees it, in a real browser | Seconds | Fill the form, click Save, see the row |
| Acceptance | "Does it do what the user needs?" Often the same tools as system tests, owned with the product owner | Varies | "A nurse can create a taper plan" |

Non-functional tests run at the system level: **performance**, **stress**, **security**, usability.

### The pyramid

```text
            /\        few     UI / end-to-end (Playwright)
           /  \               slow, real browser, many moving parts
          /----\      some    integration / API
         /      \             real HTTP pipeline, no browser
        /--------\    many    unit
       /__________\           fast, exact, cheap
```

Why this shape:

- **Speed.** 1,000 unit tests run in seconds. 1,000 UI tests can take an hour.
- **Exact failure location.** A failing unit test names the broken rule. A failing UI test only says "something on this page".
- **Stability.** UI tests depend on timing, browsers, and data. They are more often **flaky** (pass, then fail, with no code change).
- **Cost to keep.** UI changes often. Every UI change can break UI tests.

The bad shape is the **ice-cream cone**: many manual and UI tests, few unit tests. It is slow and fragile.

**Rule of thumb: test each rule at the lowest level that can see it. Test the wiring once at a higher level.**

Example: "10 mg over 3 weeks gives 6.67 mg in week 2."

- The math is a rule. It belongs in a **unit** test. Test many inputs there.
- "The screen shows 6.67, not 6.7" is display. Only the **UI** sees it. One Playwright test is enough.
- Do not test all the math through the browser. It is slow, and a failure would not tell you where the bug is.

Some teams draw a "testing trophy" instead: more integration tests, fewer unit tests. The idea is the same: choose the cheapest test that gives real confidence.

### Risk-based testing

You cannot test everything (ISTQB: "exhaustive testing is impossible"). So you rank risks:

**Risk = impact × likelihood.**

In medical software, impact means **patient harm**. A wrong dose is high impact. A wrong button color is low impact. High-risk areas get more tests, at more levels, and stricter review.

In Dose lab, the highest risks are:

1. Wrong dose in the schedule (calculator).
2. A dose above the medication limit is accepted (validation).
3. Two plans get the same reference number (a wrong plan could be given to a patient).
4. Patient data is visible without permission (security).

### What a test strategy contains

The job says "design and own end-to-end test strategies". A test strategy answers:

1. **Scope.** What we test and what we do not.
2. **Risks.** Ranked, with the reason.
3. **Levels and types.** Which tests at which level (unit, integration, UI, performance, security).
4. **Tools.** xUnit, WebApplicationFactory, Playwright, k6, CI.
5. **Environments and data.** Where tests run. How data is created and cleaned.
6. **Entry and exit criteria.** When testing can start. When a change is "done" (for example: all tests green, no open high-severity bugs).
7. **CI gates.** What blocks a merge. What runs nightly.
8. **Metrics.** Pipeline time, flaky test rate, escaped defects (bugs found after release), defects found per level. Code coverage is a hint, not a goal.

## In this repo

| Level | File | Run with |
| --- | --- | --- |
| Unit | `backend/tests/DoseLab.UnitTests/` | `dotnet test` in `backend` |
| Integration (API) | `backend/tests/DoseLab.ApiTests/` | `dotnet test` in `backend` |
| UI / system | `frontend/e2e/` | `npm run test:e2e` in `frontend` |
| Performance | `perf/` | `k6 run perf/smoke.js` |
| Pipeline | `.github/workflows/ci.yml` | GitHub Actions, on every push |

## Do

1. Run every sample test. Write down how long each level takes.
2. Read these files, in order:
   - `backend/src/DoseLab.Application/TaperScheduleCalculator.cs`
   - `backend/src/DoseLab.Application/TaperPlanService.cs`
   - `backend/src/DoseLab.Api/Controllers/TaperPlansController.cs`
   - `frontend/src/App.tsx` (only skim)
3. Write `notes/test-strategy.md`, one page, with the 8 headings above, for Dose lab. Use the four risks above. Add one more risk of your own.
4. For each sample test, write one line: "This test catches … It misses …".

## Check yourself

1. Why is a failing unit test easier to fix than a failing UI test?
2. Where do you test "a dose of exactly 80 mg is allowed for Steriva"? Why there?
3. What is the ice-cream cone, and why is it bad?
4. Name three things a test strategy contains.
5. How do you decide which features get the most tests?

## Say it in the interview

- "I put each rule at the lowest level that can see it. The dose math is unit-tested with many cases. The UI gets a few journey tests."
- "I rank risks by patient impact and likelihood. Dose calculation and dose limits get the most tests."
- "A test strategy for me covers scope, risks, levels, tools, data, exit criteria, CI gates, and metrics like flaky rate and escaped defects."
