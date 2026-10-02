# Exam 2 — Practical bug hunt

Time: 90 minutes.

Your mentor has turned on **4 hidden bugs**, in different layers of the app. The sample tests still pass. Find each bug with a test.

## Setup (your mentor does this)

Your mentor gives you a branch, for example `exam-2`. Then:

```bash
git fetch
git switch exam-2
```

Restart the API and the UI. Do not read `instructor/`, `git diff`, or `git log -p`.

## What to do

For each bug you find:

1. **A failing test** that shows the bug. Put it at the **lowest level** that can see the bug (unit, API, Playwright, or k6).
2. **A bug report** in `notes/exam-2-bugs.md`, with the template from lesson 10: title, area, severity, steps, expected, actual, evidence (test name).
3. **Layer reason.** One sentence: why this level and not a lower one.

Think about every layer. Some bugs only show under load.

At the end, commit your tests and notes and push the branch. Your mentor turns the bugs off. **Your tests must then pass.** A test that fails on correct code is a wrong test.

## Scoring (40 points, pass 30)

Per bug, 10 points:

| Points | For |
| --- | --- |
| 4 | A test that fails because of the bug |
| 2 | The test is at the lowest level that can see the bug |
| 3 | A clear bug report: steps, expected, actual, severity |
| 1 | The test passes when the bug is off |

Minus 2 points for each "bug" you report that is not a bug.

## Tips

- Start with the cheap tests: `dotnet test` with your unit and API tests from lessons 02–03.
- Then Playwright. Then k6 (restart the API before each k6 run).
- Look at boundaries, rounding, status codes, headers, security, what the screen shows after an action, speed with a lot of data, and many users at once.
- Time-box: about 20 minutes per bug. If one is stuck, move on.
