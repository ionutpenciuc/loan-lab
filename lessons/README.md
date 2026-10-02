# Study plan: 3 days to interview-ready

Target role: **Senior Test Automation Engineer** for medical software (.NET, C++ desktop, web).

You have 3 days, about 4–5 hours each. Do the lessons in order. Each lesson has:

- **Learn**: the ideas, in short form.
- **In this repo**: where you can see the idea in the code.
- **Do**: exercises. Write real tests. Run them.
- **Check yourself**: questions. Answer them out loud, without notes.
- **Say it in the interview**: sentences you can reuse.

Keep your own notes in a `notes/` folder in the repo. Interviewers like it when you show real work.

## Before you start

1. Follow the main [README](../README.md) until the app runs and all sample tests pass.
2. Fork the repo to your own GitHub account (button **Fork** on GitHub). Push your work to your fork. Then you can show it in the interview.
3. Do **not** open `instructor/`. It has the exam answers and the hidden bugs.

## Day 1 — Strategy and backend tests

| Time | Lesson |
| --- | --- |
| 45 min | [01 Test strategy and the testing pyramid](01-test-strategy-and-pyramid.md) |
| 90 min | [02 C# for testers and unit tests](02-csharp-and-unit-tests.md) |
| 75 min | [03 Integration and API tests](03-integration-and-api-tests.md) |
| 30 min | Practice bug hunt 1 (end of lesson 03) |

## Day 2 — UI, performance, pipelines

| Time | Lesson |
| --- | --- |
| 90 min | [04 UI and system tests with Playwright](04-ui-tests-with-playwright.md) |
| 75 min | [05 Performance and stress tests with k6](05-performance-and-stress-with-k6.md) |
| 45 min | [06 CI/CD pipelines](06-ci-cd-pipelines.md) |
| 30 min | Practice bug hunt 2 (end of lesson 06) |

## Day 3 — Senior topics and exams

| Time | Lesson |
| --- | --- |
| 40 min | [07 Security testing basics](07-security-testing.md) |
| 50 min | [08 AI agents in QA (Claude Code)](08-ai-in-qa.md) |
| 40 min | [09 Medical software quality, shift-left, ISTQB](09-medical-quality-shift-left-istqb.md) |
| 30 min | [10 Interview playbook](10-interview-playbook.md) |
| 45 min | [Exam 1 — theory](../exams/exam-1-theory.md) |
| 90 min | [Exam 2 — practical bug hunt](../exams/exam-2-practical.md) |
| 45 min | [Exam 3 — mock interview](../exams/exam-3-mock-interview.md) |

Day 3 is long. If needed, do Exam 3 on the evening before the interview.

You are ready when you pass all three exams. See [exams/README.md](../exams/README.md).

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
