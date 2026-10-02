# Exam 1 — Theory

Time: 45 minutes. No notes, no internet, no AI.

Write your answers in `notes/exam-1-answers.md`. For part A, write the number and the letter (for example `1 b`).

Total: 40 points. Pass: 32.

## Part A — Multiple choice (1 point each)

Choose one answer.

**1.** Why should most automated tests sit at the bottom of the pyramid?
a) Unit tests find every bug.
b) They are fast and stable, and a failure points to the exact code.
c) UI tests are not allowed in CI.
d) They need no maintenance.

**2.** A rule says the starting dose must be from 1 mg to 20 mg, inclusive, with 0.01 mg precision. Which set uses boundary value analysis best?
a) 1, 20
b) 0.99, 1, 20, 20.01
c) 0, 10, 30
d) 1, 10, 20

**3.** A test passes, then fails, then passes again with no code change. It is:
a) a regression test
b) a flaky test
c) a smoke test
d) a confirmation test

**4.** In Playwright, the best way to wait for a new row in a table is:
a) `await page.waitForTimeout(2000)`
b) `await expect(rowLocator).toBeVisible()`
c) a loop with `sleep`
d) set `retries: 5`

**5.** Which locator is the most robust?
a) `page.locator('div > table tr:nth-child(3) td')`
b) an absolute XPath from `/html/body`
c) `page.getByRole('button', { name: 'Save' })`
d) `page.locator('.btn-primary')`

**6.** "p95 response time is 250 ms" means:
a) The average is 250 ms.
b) 95% of requests took 250 ms or less.
c) 95 requests took 250 ms.
d) The slowest request took 250 ms.

**7.** A stress test:
a) checks the system at expected traffic
b) pushes beyond expected load to find the breaking point and how the system fails
c) runs for 12 hours to find leaks
d) is one user checking that the app starts

**8.** A soak (endurance) test mainly finds:
a) layout bugs
b) memory leaks and slow degradation over time
c) wrong labels
d) boundary bugs

**9.** An endpoint gets slower as the number of rows grows, because each row triggers its own database call. This is:
a) a race condition
b) the N+1 query problem
c) a deadlock
d) a cache hit

**10.** The correct status code for a successful create is:
a) 200
b) 201
c) 204
d) 302

**11.** A request has no credentials, or wrong ones. The correct status code is:
a) 400
b) 401
c) 403
d) 404

**12.** `WebApplicationFactory<Program>` gives you:
a) a real browser
b) the real ASP.NET pipeline in memory, without a network port
c) a mock of every class
d) the production database

**13.** Which pipeline order gives the fastest useful feedback?
a) UI → API → unit
b) unit → API → UI → performance smoke
c) performance → UI → unit
d) all tests in one job, in random order

**14.** In ISTQB terms, confirmation testing is:
a) checking that a fixed defect is really fixed
b) re-running all tests after any change
c) testing done by the customer
d) load testing

**15.** "Exhaustive testing is impossible" means:
a) do not test at all
b) use risk and test-design techniques to choose tests
c) only test the UI
d) automate every possible input

**16.** Severity and priority:
a) are the same thing
b) severity is the impact; priority is how soon it must be fixed
c) priority is the impact; severity is how soon
d) are set only by the tester

**17.** IEC 62304 is about:
a) web accessibility
b) the life cycle of medical device software
c) password rules
d) cloud costs

**18.** ISO 14971 is about:
a) risk management for medical devices
b) user interface colors
c) the Java language
d) data protection law

**19.** An AI tool wrote 20 green tests for existing code. What is the best next step?
a) Merge them.
b) Review the assertions, and check that each test fails when the behavior is broken.
c) Delete them.
d) Ask for 20 more.

**20.** `CLAUDE.md` in a repo is:
a) the license
b) project context and rules that Claude Code reads at the start of a session
c) a test report
d) the CI configuration

## Part B — Short answers (2 points each)

Two or three sentences each.

**21.** In Dose lab, where would you test that 10 mg over 3 weeks gives 6.67 mg in week 2? And where would you test that the screen shows `6.67`? Why both?

**22.** Name three causes of flaky UI tests, and one fix for each.

**23.** Explain load, stress, spike, and soak tests, one sentence each.

**24.** A k6 run shows p95 = 900 ms for `GET /api/taper-plans`, but only after 1,000 plans exist. With 10 plans it is 10 ms. What do you suspect, and how do you confirm it?

**25.** Why must a Playwright test that saves data use a unique name? Give one other way to keep tests independent.

**26.** What is shift-left? Give two concrete practices.

**27.** You find that `GET /api/taper-plans` returns patient names with no authentication. Give a bug title, a severity, and the OWASP category, with one sentence on why.

**28.** What is a test harness? Give one example from this repo.

**29.** What is traceability in medical software testing, and why does an auditor care?

**30.** Name two risks of AI-generated tests, and how you control each one.
