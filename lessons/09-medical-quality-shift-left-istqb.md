# 09 — Medical software quality, shift-left, and ISTQB

Time: 40 minutes.

Goal: speak the language of regulated medical software, shift-left practice, and ISTQB.

## Learn: medical software standards

You do not need to know them in depth. Know what each one is, and what it means for testing.

| Standard | What it is |
| --- | --- |
| IEC 62304 | Life cycle for medical device software: planning, requirements, design, verification, release, maintenance. Safety classes **A** (no injury possible), **B** (non-serious injury), **C** (death or serious injury). Higher class → more rigor. |
| ISO 14971 | Risk management: find hazards, estimate risk, control it, verify the control works. |
| ISO 13485 | Quality management system for medical device companies. |
| IEC 62366-1 | Usability engineering. Use errors are risks too. |
| IEC 82304-1 | Health software that is not part of a device. |
| EU MDR | European Medical Device Regulation. |
| FDA 21 CFR Part 820 / Part 11 | US quality system rules / electronic records and signatures. |

What this means for a tester:

- **Traceability.** Every requirement links to its risks, its tests, and the test results. An auditor picks a requirement and asks: "show me the evidence it was tested."
- **Verification vs validation.** Verification: "did we build it right?" (meets the spec). Validation: "did we build the right thing?" (meets the user's need).
- **Documented evidence.** Test reports kept, versioned, reproducible.
- **Risk controls are tested.** The dose maximum is a risk control. It needs a test that proves it works, at the boundary.
- **Regression on every change.** Plus change control: what changed, why, what was re-tested.
- **Tool validation.** Tools used to produce evidence (test frameworks, CI) may need to be shown fit for purpose.
- **SOUP** (Software Of Unknown Provenance): third-party libraries are tracked and assessed.

## Learn: shift-left

Move quality work **earlier**, where bugs are cheap to fix.

- Testers join refinement. Ask "how will we test this?" before code exists.
- Acceptance criteria as examples (BDD, Given / When / Then):
  ```gherkin
  Given Nervalin has a maximum daily dose of 20 mg
  When a nurse previews a taper starting at 20 mg for 4 weeks
  Then the schedule is shown
  And week 1 has a daily dose of 20.00 mg
  ```
- Testability reviews of designs (ids, logs, test hooks, seed data).
- Developers write unit tests. Testers help design the cases.
- **TDD (test-driven development):** red (write a failing test) → green (simplest code to pass) → refactor (clean up, tests stay green).
- Static analysis and code review in the pull request.
- Contract tests between frontend and backend.

Shift-**right** is the other side: monitoring, logs, and alerts in production, canary releases, feature flags.

## Learn: ISTQB Foundation vocabulary

The seven principles:

1. Testing shows the presence of defects, not their absence.
2. Exhaustive testing is impossible.
3. Early testing saves time and money.
4. Defects cluster together.
5. Tests wear out (the "pesticide paradox"): the same tests stop finding new bugs.
6. Testing is context dependent (medical software ≠ a game).
7. Absence-of-errors fallacy: bug-free but useless is still a failure.

Other words:

| Word | Meaning |
| --- | --- |
| Error → defect → failure | A person's mistake → a bug in the code → wrong behavior when it runs |
| Static vs dynamic testing | Review without running (code review, reading specs) vs running the software |
| Black-box techniques | Equivalence partitioning, boundary values, decision tables, state transitions, use cases |
| White-box techniques | Statement coverage, branch coverage |
| Experience-based | Error guessing, exploratory testing (time-boxed, with a charter) |
| Confirmation testing (re-test) | Check that a fixed bug is fixed |
| Regression testing | Check that a change did not break something else |
| Severity vs priority | How bad the impact is vs how soon to fix |
| Entry / exit criteria | When testing can start / when it is done |
| Test basis, test condition, test case | What you test from (spec) / what to test / the concrete steps and expected result |

## Do

1. **Traceability matrix.** In `notes/traceability.md`, make a table for these requirements: requirement id, risk, test(s) that cover it (file and test name), level, result.
   - REQ-1 The schedule lowers the dose by an equal step each week; the last week keeps the rounding remainder.
   - REQ-2 The starting dose must be between 1 mg and the medication maximum (inclusive).
   - REQ-3 The number of weeks must be from 1 to 52.
   - REQ-4 Only callers with a valid API key can save a plan.
   - REQ-5 Every saved plan has a unique reference number.
   - REQ-6 A saved plan appears in the list at once.
   Mark gaps where no test exists yet.
2. **Gherkin.** Write three acceptance scenarios for "save a taper plan" (one happy path, two unhappy).
3. **TDD.** New rule: *"A Nervalin plan may last at most 12 weeks."* On a new branch (`git switch -c tdd-practice`):
   - Red: write a failing unit test in `TaperPlanServiceShould`.
   - Green: change `ValidateDose` in `TaperPlanService.cs` with the smallest change.
   - Refactor. Run all tests.
   - Go back to `main` afterwards (`git switch main`). The hidden-bug kit expects the original code.

## Check yourself

1. What are the IEC 62304 safety classes?
2. What is traceability, and why does an auditor care?
3. Verification vs validation?
4. What is TDD? What are its three steps?
5. Name four ISTQB principles.
6. Regression vs confirmation testing?

## Say it in the interview

- "In medical software, every requirement and risk control traces to tests and evidence. The dose maximum is a risk control, so it gets boundary tests at unit and API level."
- "Shift-left for me means examples in refinement, testability reviews, and developers and testers designing unit tests together."
- "I know the ISTQB vocabulary and techniques, and I apply them: boundaries, decision tables, exploratory charters."
