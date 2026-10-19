# Study plan: 7 days to interview-ready

Target role: **Senior Test Automation Engineer** for medical software (.NET, C++ desktop, web).

You have 7 days, about 4–5 hours each. Do the lessons in the order of the plan (not the lesson numbers). Each lesson has:

- **Learn**: the ideas, in short form.
- **In this repo**: where you can see the idea in the code.
- **Do**: exercises. Write real tests. Run them.
- **Check yourself**: questions. Answer them out loud, without notes.
- **Say it in the interview**: sentences you can reuse.

Keep your own notes in a `notes/` folder in the repo. Interviewers like it when you show real work.

## Before you start

1. Install the basics: [tools/windows-setup.md](../tools/windows-setup.md). Other tools have their own guide in [tools/](../tools/README.md); install them when a lesson needs them.
2. Follow the main [README](../README.md) until the app runs and all sample tests pass.
3. Fork the repo to your own GitHub account (button **Fork** on GitHub). Push your work to your fork. Then you can show it in the interview.
4. Do **not** open `instructor/`. It has the exam answers and the hidden bugs.

## Exercises in the code

Many lessons have exercises inside the code, marked with `TODO(L<lesson>-<n>)`, for example `TODO(L11-3)`. Exercise tests are skipped until you start them. For each one:

1. Remove `Skip = "..."` from the test.
2. Run it. See it **fail**.
3. Write the missing code. Run it. See it **pass**.

Find all exercises of a lesson: `git grep "TODO(L11"`. Find all you have left: `git grep "TODO(L"`.

## Day 1 — .NET basics and strategy

| Time | Lesson |
| --- | --- |
| 60 min | Setup: [tools/windows-setup.md](../tools/windows-setup.md) and the main README |
| 120 min | [00 .NET and C# fundamentals](00-dotnet-fundamentals.md) |
| 45 min | [01 Test strategy and the testing pyramid](01-test-strategy-and-pyramid.md) |

## Day 2 — Unit and API tests

| Time | Lesson |
| --- | --- |
| 90 min | [02 C# for testers and unit tests](02-csharp-and-unit-tests.md) |
| 75 min | [03 Integration and API tests](03-integration-and-api-tests.md) |
| 30 min | Practice bug hunt 1 (end of lesson 03) |
| 30 min | Install Docker Desktop for tomorrow: [tools/docker-and-postgres.md](../tools/docker-and-postgres.md) |

## Day 3 — Test harnesses and real dependencies

| Time | Lesson |
| --- | --- |
| 120 min | [11 Test harness engineering](11-test-harness-engineering.md) |
| 90 min | [12 Docker, PostgreSQL and Testcontainers](12-real-dependencies-docker-testcontainers.md) |

## Day 4 — UI: web and desktop, and pipelines

| Time | Lesson |
| --- | --- |
| 90 min | [04 UI and system tests with Playwright](04-ui-tests-with-playwright.md) |
| 120 min | [15 Desktop UI automation, Ranorex, Cypress](15-desktop-ui-automation.md) |
| 45 min | [06 CI/CD pipelines](06-ci-cd-pipelines.md) |

## Day 5 — Performance and stress

| Time | Lesson |
| --- | --- |
| 75 min | [05 Performance and stress tests with k6](05-performance-and-stress-with-k6.md) |
| 90 min | [13 Finding bottlenecks in .NET](13-dotnet-performance-profiling.md) |
| 30 min | Practice bug hunt 2 (end of lesson 06) |
| 40 min | [07 Security testing basics](07-security-testing.md) |

## Day 6 — Penetration testing and AI

| Time | Lesson |
| --- | --- |
| 150 min | [14 Penetration testing lab](14-penetration-testing-lab.md) |
| 50 min | [08 AI agents in QA (Claude Code)](08-ai-in-qa.md) |
| 120 min | [16 Building AI QA workflows](16-building-ai-qa-workflows.md) |

Day 6 is long. If needed, move lesson 16 steps 5–7 to the morning of day 7.

## Day 7 — Medical quality, interview, exams

| Time | Lesson |
| --- | --- |
| 40 min | [09 Medical software quality, shift-left, ISTQB](09-medical-quality-shift-left-istqb.md) |
| 30 min | [10 Interview playbook](10-interview-playbook.md) |
| 60 min | [Exam 1 — theory](../exams/exam-1-theory.md) (parts A, B and C) |
| 90 min | [Exam 2 — practical bug hunt](../exams/exam-2-practical.md) |
| 50 min | [Exam 3 — mock interview](../exams/exam-3-mock-interview.md) |

You are ready when you pass all three exams. See [exams/README.md](../exams/README.md).

## Short on time?

Keep the core: 00–06, 11, 14 (steps 1–4 and 6), 16 (steps 1, 2, 6). These can be shortened: 12 steps 5–7, 13 steps 5–6, 15 step 7 and the Cypress try-out, 14 step 5.

## How the hidden bugs work

Your mentor can turn on small bugs in the app. The sample tests stay green. Only a good new test finds a bug. This is how real QA work feels: the existing tests pass, but something is wrong.

You can also practice alone:

```bash
instructor/bug.sh random 1   # turns on one random bug; it does not say which
```

Run all your tests. If all stay green, your tests missed the bug. Add tests until one fails. Then check:

```bash
instructor/bug.sh list       # shows which bug was on
instructor/bug.sh reset      # turns every bug off
```

Restart the API and the UI after you turn a bug on or off. On Windows, run the script in Git Bash.
