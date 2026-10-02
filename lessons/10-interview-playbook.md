# 10 — Interview playbook

Time: 30 minutes, plus practice.

Goal: tell your story, answer the likely questions, and show your work.

## Your story (60 seconds)

Write it in `notes/my-story.md`, then say it out loud 5 times.

1. Who you are: years in testing, the domains you know.
2. What you do best as a tester: finding the risky cases, clear bug reports, working with developers.
3. What you built recently: "I built a full test suite for a .NET and React medical-style app: unit, API, Playwright, k6 load and stress tests, a CI pipeline, and AI skills for test work." Show your fork.
4. Why this role: medical software, where quality protects patients.

Be honest about experience. Show that you learn fast and think in risks.

## STAR stories

Prepare 5 stories. **S**ituation, **T**ask, **A**ction, **R**esult (with a number if possible).

1. A bug you found that really mattered.
2. A disagreement with a developer or product owner, and how you solved it.
3. A process you improved (faster tests, better bug reports, fewer escaped bugs).
4. A time you learned something new fast (this lab is one).
5. A time requirements were unclear, and what you did.

## Bug report template

```text
Title:     Saving a plan without an API key is accepted
Area:      API, POST /api/taper-plans
Severity:  High (unauthorized changes to prescriptions)   Priority: P1
Steps:     1. curl -X POST http://localhost:5080/api/taper-plans -H "Content-Type: application/json"
              -d '{"patientName":"X","medicationCode":"STR","startingDailyDoseMg":40,"weekCount":4}'
Expected:  401 Unauthorized, nothing saved
Actual:    201 Created, plan saved
Evidence:  failing test TaperPlansApiShould.RejectASaveWithoutAnApiKey, response body
Notes:     A wrong key is still rejected. Only a missing header gets through.
```

## Likely questions, and where you learned the answer

| Question | Lesson |
| --- | --- |
| How would you design a test strategy for our product? | 01 |
| Explain the testing pyramid. When do you break it? | 01 |
| How do you choose test cases for a numeric limit? | 02 |
| How do you test an API? What do you check? | 03 |
| How do you structure a large Playwright suite? How do you stop flaky tests? | 04 |
| Walk me through a performance test. What do you report? | 05 |
| How do you find a bottleneck? | 05 |
| Design a pipeline for us. What runs when? | 06 |
| How do you test security? | 07 |
| How do you use AI agents in QA? How do you trust the output? | 08 |
| What is different about medical software? | 09 |
| TDD and shift-left: how do you work with developers? | 09 |

### Topics outside this lab

**C++ desktop apps.** The pyramid is the same.

- Unit tests: GoogleTest or Catch2.
- UI automation for Windows desktop: Ranorex (named in the ad), FlaUI (C#, Windows UI Automation), WinAppDriver / Appium.
- Same rules as web: stable element ids (automation ids), no fixed waits, page objects ("screen objects").

**Ranorex** is a commercial tool for desktop, web, and mobile UI tests, with a recorder and C# code. **Cypress** is a JavaScript web test tool, similar to Playwright. It runs inside the browser, while Playwright drives the browser from outside.

If you do not know something: say so, then say how you would find out. "I have not used Ranorex yet. I'd start with its C# API, because I write my tests in C# already."

## Questions to ask them

1. What does your test pyramid look like today? Where is it weakest?
2. How long does the pipeline take, and how flaky is it?
3. Which safety class is the software, and how do you handle traceability?
4. How does the QA team use AI agents today? What would you like this role to build?
5. Who owns the test strategy: QA, developers, or both?
6. What would success look like in my first 6 months?

## The day before

- Pass all three exams (see `exams/`).
- Push your fork. Clean up `notes/`. Make the README of your fork say what you added.
- Practice your story and 5 STAR stories out loud.
- Prepare a 3-minute demo: run `dotnet test`, run one Playwright test in UI mode, show a k6 summary, show a skill file.
