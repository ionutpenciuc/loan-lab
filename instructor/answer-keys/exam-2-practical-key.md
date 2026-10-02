# Exam 2 — Answer key

Use the bug table in `instructor/README.md` to know what is on. For each bug, score 10 points.

## What a full-score answer looks like

### 01 — Rounding remainder (unit)

- Test: calculator with an uneven split (10 mg / 3 weeks), asserts 6.67 and 3.34 (or asserts that the steps add up to the starting dose).
- Lowest layer: unit. A Playwright test that sees 6.66 on screen finds it too, but gets 0 of 2 layer points.
- Report: "Taper schedule: the last week does not keep the rounding remainder (10 mg / 3 weeks gives 6.66 / 3.33)". Severity high: wrong dose for the patient.

### 02 — Maximum dose boundary (unit)

- Test: service `Create` or `Preview` with a dose **equal** to the maximum (80 mg for STR, 40 for CLM, 20 for NRV) is accepted.
- Lowest layer: unit. An API test that sends exactly the maximum also finds it (1 of 2 layer points).
- Report: "A starting dose equal to the medication maximum is rejected". Severity medium: valid prescriptions blocked, staff may work around the system.

### 03 — Create status code (API)

- Test: POST with key → expect 201 and a `Location` header.
- Lowest layer: API. No unit test sees HTTP status codes; the UI ignores it.
- Report: "POST /api/taper-plans returns 200 without Location instead of 201 Created". Severity low–medium: breaks API clients and contracts.

### 04 — Missing API key accepted (API)

- Test: POST **without** the header → expect 401, and the plan is not in the list.
- Note: a "wrong key" test alone passes. She needs the missing-header case.
- Lowest layer: API.
- Report: "Saving a plan without an API key is accepted". Severity high: anyone can create prescriptions.

### 05 — List not refreshed after save (Playwright)

- Test: fill the form, save, expect the new (unique) name in the table **without** `page.reload()`.
- If her test reloads the page before checking, it does not find the bug: give 0 for that bug.
- Lowest layer: UI. The API is correct.
- Report: "New plan does not appear in the list after Save until the page is reloaded". Severity medium: staff may save twice and create duplicates.

### 06 — Doses shown with 1 decimal (Playwright)

- Test: preview an uneven taper, expect `6.67` (or `40.00` in the list).
- Lowest layer: UI. An API test shows correct data; that is good evidence that the bug is in the frontend.
- Report: "Taper schedule shows doses rounded to 1 decimal (6.7 instead of 6.67)". Severity high: the screen shows a dose that differs from the plan.

### 07 — List slows down with data (k6 load)

- Test: create a few hundred plans, then load `GET /api/taper-plans` with a p95 threshold. With the bug, p95 grows to several hundred ms.
- Full points also if she measures the same thing with a timed loop in an API test, but k6 with a threshold is the expected answer.
- Report should give numbers: p95 with 2 plans vs with 200, and the suspicion "per-row lookup / N+1". Severity medium.

### 08 — Duplicate reference numbers under load (k6 stress)

- Test: many users save plans at the same time; afterwards, check the reference numbers are unique (threshold on a counter, or a check in `teardown`).
- Full points also for a C# test that saves in parallel (`Parallel.For` / `Task.WhenAll`) through the API and checks uniqueness. That is a valid integration-level concurrency test.
- Report: "Concurrent saves produce duplicate plan reference numbers". Severity high: two patients' plans can be confused.

## Last check

Turn the bugs off (`instructor/bug.sh reset`), restart, and run all her tests. Each test that fails now loses its 1 point ("passes when the bug is off").

## Pass

30 of 40. Below that, note which layers she missed and have her redo those lessons.
