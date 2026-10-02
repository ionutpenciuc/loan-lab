---
name: triage-test-failure
description: Classifies a failing test (product bug, test bug, environment, or flaky) and drafts a bug report. Use when a test, CI job, Playwright run, or k6 threshold fails and the cause is not yet known.
---

# Triage a test failure

1. Collect evidence: the failure message, the test name and file, the command, and (for Playwright) the trace or screenshot in `frontend/test-results/`. For k6, the threshold that failed and the numbers.
2. Re-run the single test 3 times.
   - Passes sometimes → likely **flaky**. Look for fixed waits, shared data, order dependence, weak locators, timing.
3. Check the environment:
   - Is the API running on the expected port? Is an old API or UI still running (stale code)? Did the API restart after a code change?
   - For k6: was the API restarted before the run (data grows in memory)?
   - Problem here → **environment**.
4. Check the test against the rule in `CLAUDE.md` (Domain rules).
   - The test's expected value breaks the rule → **test bug**. Fix the test.
5. The test matches the rule and the app does not → **product bug**. Do not change product code. Write a report:

```text
Title:     <what is wrong, in one line>
Area:      <layer and endpoint / screen / class>
Severity:  <High / Medium / Low> because <impact on the patient or the user>
Steps:     <exact steps or command>
Expected:  <from the rule>
Actual:    <what happened, with values>
Evidence:  <test name, output, trace>
```

6. Say at which level the bug should be caught, and whether a lower-level test is missing.

## Output

One line: `Classification: <product bug | test bug | environment | flaky>`, then the reasons, then the report (for product bugs) or the fix (for others).
