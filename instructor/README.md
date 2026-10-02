# Instructor guide

This folder is for the mentor. It has the hidden bugs, the answer keys, and reference tests. The student agrees not to read it.

## Contents

| Path | What |
| --- | --- |
| `bug.sh` | Turns bugs on and off |
| `bugs/01.patch` … `08.patch` | The bugs, as small code changes |
| `verify.sh` | Proves every bug still works as designed (about 7 minutes) |
| `solutions/` | Reference tests that catch every bug |
| `answer-keys/` | Keys for the three exams |

## The bugs

Every bug keeps the sample tests green. Each is caught by a test at one layer.

| Id | Lowest layer that catches it | File changed | What is wrong | Reference test |
| --- | --- | --- | --- | --- |
| 01 | Unit (calculator) | `TaperScheduleCalculator.cs` | Each week is rounded on its own instead of using one fixed step. 10 mg / 3 weeks gives 6.66 and 3.33, not 6.67 and 3.34. The steps no longer add up to the starting dose. | `TaperScheduleCalculatorSolutionShould.KeepTheRoundingRemainderInTheLastWeek` |
| 02 | Unit (service) | `TaperPlanService.cs` | Off-by-one at the maximum: a dose **equal** to the medication maximum is rejected (`>=` instead of `>`). | `TaperPlanServiceSolutionShould.AcceptAStartingDoseEqualToTheMedicationMaximum` |
| 03 | API | `TaperPlansController.cs` | Create returns 200 with no `Location` header, instead of 201 + `Location`. The UI does not notice. | `TaperPlansApiSolutionShould.ReturnCreatedWithALocationThatFindsThePlan` |
| 04 | API (security) | `RequireApiKeyAttribute.cs` | A **missing** `X-Api-Key` header is accepted. A wrong key is still rejected, so a "wrong key" test alone does not find it. | `TaperPlansApiSolutionShould.RejectASaveWithoutAnApiKey` |
| 05 | UI (Playwright) | `App.tsx` | After Save, the form closes but the list is not reloaded. The new plan appears only after a page reload. | `shows a saved plan in the list without a reload` |
| 06 | UI (Playwright) | `App.tsx` | Doses are shown with 1 decimal (`6.7`, `40.0`). The API is correct. | `previews an uneven taper with two decimals` |
| 07 | Performance (k6 load) | `TaperPlanService.cs` | The list makes one catalog call per plan (N+1). Fine with 2 plans; with 200 plans, p95 goes from under 10 ms to several hundred ms. | `solutions/perf/list-latency.js` |
| 08 | Stress (k6, concurrency) | `InMemoryTaperPlanRepository.cs` | The next reference number is read outside the lock. Under parallel saves, plans get **duplicate** reference numbers. One user never sees it. | `solutions/perf/stress-create.js` |

Bugs 01 and 06 also show in the UI, and bug 02 also shows through the API. A unit test is still the right answer for 01 and 02: it is the lowest level. Give full layer points for the lowest level only.

## Run an exam 2 bug hunt

Pick one bug from each group so every layer is covered: one of 01/02, one of 03/04, one of 05/06, one of 07/08.

```bash
git switch main && git pull
git switch -c exam-2
instructor/bug.sh on 02 04 05 08
git commit -qam "Exam 2 setup"
git push -u origin exam-2
```

If she works on her fork, push the branch there instead, or let her pull it from yours.

After the exam:

```bash
git switch exam-2 && git pull         # get her tests and notes
instructor/bug.sh reset               # turn the bugs off
# restart API and UI, then run her tests: they must pass now
```

Grade with `answer-keys/exam-2-practical-key.md`.

For a second attempt, choose a different set, for example `01 03 06 07`.

## Practice without the mentor

She can run `instructor/bug.sh random 1` herself. The script does not say which bug is on, and the patch files have neutral names.

## Check the kit after you change the app

If you change the app code, a patch can stop applying. Run:

```bash
instructor/verify.sh            # needs .NET 10, Node.js, Playwright Chromium, k6
```

It checks, for every bug: the sample tests still pass, and the reference test for that layer fails. Ports 5191 and 5192 are used, so it does not disturb a running app. To rebuild a patch: change the code, run `git diff -- <file> > instructor/bugs/NN.patch`, then `git checkout -- <file>`.

## Lesson exercise answers

The reference tests in `solutions/` also answer the lesson exercises:

| Lesson | Reference |
| --- | --- |
| 02 Unit | `solutions/backend/TaperScheduleCalculatorSolutionShould.cs`, `TaperPlanServiceSolutionShould.cs` |
| 03 API | `solutions/backend/TaperPlansApiSolutionShould.cs` |
| 04 Playwright | `solutions/e2e/taper-plans.solution.spec.ts` |
| 05 k6 | `solutions/perf/list-latency.js`, `solutions/perf/stress-create.js` |

To run a reference test, copy it into the matching test folder (see the first line of each file).
