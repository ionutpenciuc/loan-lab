# Exam 3 — Mock interview key

Score each answer 1–4:

| Score | Meaning |
| --- | --- |
| 1 | Wrong, or no real answer |
| 2 | Right words, no example |
| 3 | Right, with a concrete example (Dose lab counts) |
| 4 | Right, example, plus trade-offs or numbers, and links to medical risk |

Pass: average 3 or more, no 1 on questions 2, 3, 6, 7, 9, 10.

Ask at least one follow-up per answer ("why?", "what would you do if…?"). Below: what a 4 includes.

## 1. Tell me about yourself

60–90 seconds. Experience, strengths, what she built (the lab, with all layers), why medical software. Honest about the move from manual to automation, framed as growth.

## 2. Test strategy for a medical web app

Starts from **risks** (patient harm × likelihood), not tools. Scope; levels (unit, API, UI, performance, security) with what goes where; tools; environments and test data; entry/exit criteria; CI gates; metrics (escaped defects, flaky rate, pipeline time). Mentions traceability and evidence. Example: dose rules → many unit and API tests, few UI journeys.

## 3. Testing pyramid; when to break it

Many fast unit tests, fewer integration, few UI. Why: speed, exact failure location, stability, cost. Ice-cream cone is the anti-pattern. Break it when: legacy code with no seams (start with UI/API tests as a safety net, then push down); thin CRUD apps (more API-level, "trophy"); critical journeys that need end-to-end proof.

## 4. Test cases for "dose from 1 to 20 mg"

Equivalence partitions (below, inside, above) and boundary values: 0.99, 1, 20, 20.01 (precision matters). Also: invalid types, empty, negative, very large, decimals with more digits. Where: unit level, with a `[Theory]`. One API test for the error response.

## 5. Testing a REST API

Status codes, headers (`Location`), body/contract, error messages, side effects (nothing saved on rejection), auth (missing / wrong / right), not found, input limits. Tools: WebApplicationFactory (in memory), Postman/REST client for exploring, contract tests. Isolation of data.

## 6. 400 tests, 50 minutes, 5% random failures

Measure first: which tests fail, how often (flaky report). Quarantine flaky tests out of the blocking path; fix causes (waits, data, locators, order). Speed: parallel workers and sharding; move checks down the pyramid (many UI tests duplicate API/unit coverage); create data through the API; smoke subset on PRs, full suite on merge/nightly. Track flaky rate and duration as metrics.

## 7. Performance test for an API

Goal and targets first (SLOs, expected traffic). Scenarios: smoke, load, stress, maybe spike/soak. Realistic data volume and think time. Measure p95/p99, throughput, error rate, resource usage. Thresholds as gates. Report: environment, scenario, results vs targets, bottlenecks, recommendation. Example: list endpoint with 200 plans; duplicate references under 50 users.

## 8. Finding a bottleneck

Reproduce, then narrow down: does it depend on data size (N+1, index, paging), on concurrency (locks, thread pool, connection pool), or on time (leak)? Tools: profiler (dotTrace), `dotnet-counters`, `dotnet-trace`, APM, database query logs. Change one thing, re-measure against the baseline. Work with developers on the fix, then confirm with the same test.

## 9. CI pipeline design

PR: build, unit, API tests, lint, a UI smoke subset, dependency scan — fast (target under 10–15 min). Merge: full UI suite, perf smoke, deploy to a test environment. Nightly: cross-browser, load/stress/soak, DAST scan. Artifacts: reports, traces, screenshots. Gates and branch protection. GitLab terms: stages, jobs, artifacts, rules, schedules. Flaky tests out of the blocking path.

## 10. AI agents in QA

Concrete uses: generate tests from acceptance criteria, find missing cases, review tests, triage failures, maintain locators, summarize traces. Context engineering: short `CLAUDE.md`, skills for repeated work, subagents for review, MCP for tools, headless runs in CI. Trust: give rules and expected values; red/green or mutation check; human review; never real patient data. For the QA community: shared skills and a reviewer agent so everyone uses the same quality bar. A 4 shows a real file she wrote (her `write-playwright-test` skill).

## 11. What is different about medical software

Patient safety. IEC 62304 (safety classes A/B/C), ISO 14971 risk management, ISO 13485 QMS, maybe FDA/MDR. Traceability from requirements and risk controls to tests and results; documented, reproducible evidence; verification and validation; change control and regression; tool validation; SOUP.

## 12. A bug that mattered (STAR)

Clear situation, her own actions, a result with impact (ideally a number). Shows judgment: how she found it, how she reported it, how she made sure it did not come back (a test).

## Extra questions

- **Security without being a pen tester:** auth and input tests at API level, dependency and secret scans in CI, ZAP baseline nightly, OWASP Top 10 as a checklist; written scope before any pen test.
- **TDD / shift-left:** red-green-refactor; testers help design cases before code; examples in refinement.
- **decimal vs double:** decimal is exact base-10; double has binary rounding errors (0.1 + 0.2). Doses and money need exact values.
- **C++ desktop:** same pyramid; GoogleTest/Catch2 for units; Ranorex, FlaUI, WinAppDriver for UI; stable automation ids.
- **"Works on my machine":** reproduce together; compare environment, data, versions; give exact steps and evidence; make the environment reproducible (containers, CI).
- **Gaps:** honest list (e.g. Ranorex, C++), with a plan.
