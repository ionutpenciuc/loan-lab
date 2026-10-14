# Exam 1 — Answer key

Parts A and B: 40, pass 32. Part C: 20, pass 14. Both must pass.

## Part A (1 point each)

| Q | Answer | Q | Answer |
| --- | --- | --- | --- |
| 1 | b | 11 | b (403 = known user, not allowed) |
| 2 | b | 12 | b |
| 3 | b | 13 | b |
| 4 | b | 14 | a (b is regression testing) |
| 5 | c | 15 | b |
| 6 | b | 16 | b |
| 7 | b | 17 | b |
| 8 | b | 18 | a |
| 9 | b | 19 | b |
| 10 | b | 20 | b |

## Part B (2 points each)

Give 2 for a correct answer with the key point, 1 for partly right, 0 for wrong.

**21.** Unit test for the calculation (1 point): it is a rule, fast, exact, many cases. Playwright (or a frontend component test) for the display (1 point): formatting is a different bug, only the screen sees it. One UI test is enough; do not repeat the math there.

**22.** Any three, each with a fix:
- Fixed waits → web-first assertions / auto-wait.
- Shared or leftover data → unique data, setup per test, API setup, clean state.
- Order dependence → each test sets up its own state.
- Weak CSS/XPath locators → role / label / test id locators.
- Slow or unstable backend → mock the network for UI-only checks; stable test environment.
- Time, dates, animations → fixed clock, disable animations.
3 good pairs = 2 points. 2 pairs = 1 point.

**23.** 0.5 each:
- Load: expected traffic, check targets.
- Stress: above expected, find the breaking point and the failure mode.
- Spike: a sudden jump in users.
- Soak: normal load for hours, find leaks and slow degradation.

**24.** Suspect data-dependent cost: N+1 calls (one query per row), missing index, no paging, large serialization (1 point). Confirm: compare timings at several data sizes; profile (dotTrace / `dotnet-trace`), count database calls in logs or APM, check the code path for per-row calls (1 point). (This is bug 07.)

**25.** Data persists between tests and runs (the API keeps data in memory; tests may run in parallel), so a fixed name can match an old row and give a false pass, or clash (1 point). Other ways: create data per test through the API; reset or fresh environment per run or worker; clean up after the test; isolated test database (1 point).

**26.** Shift-left = do quality work earlier in the life cycle, where defects are cheaper (1 point). Two practices, e.g.: testers in refinement, acceptance criteria as examples (BDD), testability reviews, TDD, unit tests in the same pull request, static analysis, contract tests (1 point).

**27.** Title like "Patient data readable without authentication on GET /api/taper-plans"; severity High (or Critical); OWASP A01 Broken Access Control (1 point). Why: exposes personal health data to anyone; GDPR/HIPAA impact (1 point).

**28.** Code and setup that start, configure, and feed the system under test, and collect results (1 point). Example: `WebApplicationFactory` / a custom factory that swaps services; Playwright `webServer` config that starts API and UI; a page object; the k6 `setup()` that creates data; `SimulatedLatency.None` (1 point).

**29.** A documented link from each requirement (and risk control) to its tests and results (1 point). The auditor needs evidence that every requirement and every risk control was verified, and what was re-tested after a change (1 point).

**30.** Any two (1 point each):
- Tests that pass on buggy code (copy current behavior) → give the rule and expected values; check tests fail when behavior breaks (red/green, mutation, the bug kit).
- Invented APIs or wrong facts → run the tests, review every diff.
- Weak assertions → review checklist / reviewer agent.
- Changes to product code to make tests pass → rules in `CLAUDE.md`, review.
- Patient data leaked into prompts → synthetic data only, follow policy.
- Accountability in regulated work → a human reviews and owns each test, traceability kept.

## Part C (2 points each)

**31.** A `WebApplicationFactory<Program>` (or a subclass like `DoseLabApiFactory`) with `builder.UseSetting("Lab:ApiKey", ...)` in `ConfigureWebHost` / `WithWebHostBuilder` (1 point each: the class, the method). Environment variable `Lab__ApiKey` also accepted for 1 point.

**32.** Stub returns fixed answers; mock also checks that calls happened (1 point). Bad idea when the call is not the requirement: it mirrors the implementation, breaks on refactors, finds no bugs (over-specification) (1 point).

**33.** The code runs, but many results are not checked: weak or missing assertions (1 point). Look at survived mutants and write tests that kill them, starting with risky code (dose rules) (1 point).

**34.** Shared state / order dependence: another test changes shared data (1 point). Reproduce alone vs together and read what the tests share (fixture, data, static state); quarantine it if it is in the blocking path (1 point).

**35.** Real behavior (types, SQL, constraints) and isolation: fresh database per test, no leftover data, same in CI (1 point). Cost: needs Docker; seconds of start-up; more CI resources (1 point).

**36.** Thread pool starvation from blocking calls (sync I/O, `Thread.Sleep`, `.Result`) (2 points; 1 if only "too many requests").

**37.** The scanner's requests failed validation earlier (unknown medication, missing key), so they never reached the SQL; scanners need valid business input (1 point). Findings: SQL injection (A03) and information disclosure through the error response / stack trace (A05) (1 point).

**38.** Through Windows UI Automation, by AutomationId (or accessible name), not positions (1 point). Ask for stable automation ids / accessible names on every control (1 point).

**39.** A hook when it must always happen, enforced by the harness; `CLAUDE.md` is only advice (1 point). Example: run unit tests after every test edit; block edits to product code; block `git push` when tests are red (1 point).

**40.** An eval: run the AI-written tests against known seeded bugs (the bug kit) and count how many are caught; they must also pass with no bugs (1 point). Rerun after each skill change and compare scores; also review assertions (1 point).
