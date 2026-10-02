# 04 — UI and system tests with Playwright

Time: 90 minutes.

Goal: write stable Playwright tests, structure them with page objects, and know how to fight flakiness.

## Learn

### What Playwright gives you

- Real browsers: Chromium, Firefox, WebKit (Safari engine).
- **Auto-wait.** Before a click, Playwright waits until the element is visible, enabled, and stable.
- **Web-first assertions.** `await expect(locator).toBeVisible()` retries until it passes or times out (5 s by default). You never need `sleep`.
- **Isolation.** Each test gets a fresh browser context (new cookies, new storage).
- **Tools.** Code generator, UI mode, trace viewer, screenshots and video on failure.
- Same library for .NET: `Microsoft.Playwright` (C#). Same ideas, different syntax.

### Locators: from best to worst

| Locator | Example | Why |
| --- | --- | --- |
| By role and name | `page.getByRole('button', { name: 'Save' })` | What users and screen readers see. Survives CSS changes. |
| By label | `page.getByLabel('Patient name')` | Form fields |
| By text | `page.getByText('Taper plans')` | Visible text |
| By test id | `page.getByTestId('save')` | When there is no good role or label. Needs `data-testid` in the code |
| CSS / XPath | `page.locator('div > table tr:nth-child(3)')` | Breaks when the layout changes. Avoid. |

Scope locators inside a part of the page:

```ts
const plans = page.getByRole('table', { name: 'Taper plans' })
await expect(plans.getByRole('cell', { name: 'Maria Ionescu' })).toBeVisible()
```

### Flaky tests: causes and fixes

| Cause | Fix |
| --- | --- |
| Fixed waits (`waitForTimeout(2000)`) | Wait on a result: `expect(...).toBeVisible()` |
| Shared data between tests | Unique names (`Elena ${Date.now()}`), or create data per test |
| Order dependence (test B needs test A) | Each test sets up its own state |
| Weak locators | Role / label locators |
| Slow or random backend | Mock the network for UI-only checks (`page.route`) |
| Animations, time zones, dates | Disable animations, fix the clock (`page.clock`) |

Retries in CI can hide flakiness. Treat "passed on retry" as a bug to fix. Move a known flaky test to **quarantine** (a separate, non-blocking run) until fixed.

### Page Object Model (POM)

Put locators and actions for a page in one class. Tests read like steps. When the UI changes, you fix one place.

```ts
import { expect, type Page } from '@playwright/test'

export class TaperPlansPage {
  constructor(private readonly page: Page) {}

  plans() { return this.page.getByRole('table', { name: 'Taper plans' }) }
  schedule() { return this.page.getByRole('table', { name: 'Taper schedule' }) }

  async open() {
    await this.page.goto('/')
    await expect(this.plans()).toBeVisible()
  }

  async fillForm(patient: string, medicationCode: string, dose: string, weeks: string) {
    await this.page.getByRole('button', { name: 'Add taper plan' }).click()
    await this.page.getByLabel('Patient name').fill(patient)
    await this.page.getByLabel('Medication').selectOption(medicationCode)
    await this.page.getByLabel('Starting daily dose (mg)').fill(dose)
    await this.page.getByLabel('Number of weeks').fill(weeks)
  }
}
```

For a large suite (hundreds of tests): page objects or fixtures, test data builders, tags (`@smoke`, `@regression`), parallel workers, sharding in CI, and one `playwright.config.ts` per environment.

### Set up data through the API, not the UI

Clicking through a form to create data is slow. Create it with Playwright's `request` and only test the screen you care about:

```ts
test('shows a plan created through the API', async ({ page, request }) => {
  const patient = `Api Patient ${Date.now()}`
  await request.post('http://localhost:5080/api/taper-plans', {
    headers: { 'X-Api-Key': 'lab-dev-key' },
    data: { patientName: patient, medicationCode: 'STR', startingDailyDoseMg: 40, weekCount: 4 },
  })
  await page.goto('/')
  await expect(page.getByRole('cell', { name: patient })).toBeVisible()
})
```

### Mock the network for error states

```ts
await page.route('**/api/taper-plans', (route) => route.fulfill({ status: 500, body: '{}' }))
await page.goto('/')
await expect(page.getByRole('alert')).toContainText('Could not load taper plans')
```

## In this repo

- Sample: `frontend/e2e/taper-plans.spec.ts`.
- Config: `frontend/playwright.config.ts`. It starts the API and the UI for you, or reuses them if they already run.

Commands, from `frontend`:

```bash
npm run test:e2e                         # all tests, headless
npx playwright test --ui                 # UI mode: watch, pick, debug
npx playwright test --headed             # see the browser
npx playwright test -g "seeded"          # only tests whose name matches
npx playwright codegen http://localhost:5173   # record clicks into code
npx playwright show-trace test-results/<folder>/trace.zip
```

## Do

Write all tests in `frontend/e2e/taper-plans.spec.ts`.

1. **Explore with codegen.** Record: open the form, fill it, preview. Read the generated code. Do not keep it as is. Rewrite it with good locators.
2. **Preview an uneven taper.** Steriva is easy (40 / 4). Use Calmafen (`CLM`), 10 mg, 3 weeks. Assert 3 body rows (`schedule.locator('tbody tr')`), week 2 shows `6.67`, and week 3 shows `3.34`.
3. **Save shows the new row.** Use a unique patient name. Click Save. Assert the name appears in the plans table **without a reload**.
4. **Server error message.** Nervalin (`NRV`), 25 mg. Click Preview. Assert the alert text is `Starting daily dose must not exceed 20 mg for Nervalin.` and no schedule table is shown.
5. **Page object.** Move locators and actions into a `TaperPlansPage` class (in `frontend/e2e/pages/taper-plans-page.ts`). Refactor tests 2–4 to use it.
6. **Network mock.** Make `GET /api/taper-plans` return 500. Assert the error alert.
7. **API setup.** Create a plan with `request`, then check the list.
8. **Cross-browser.** Add a `firefox` project in `playwright.config.ts`. Run `npx playwright install firefox` first.

## Check yourself

1. Why is `getByRole` better than a CSS selector?
2. What does auto-wait do? What does a web-first assertion do?
3. Give three causes of flaky tests, with a fix for each.
4. Why use unique names when a test saves data?
5. Why create test data through the API?
6. When would you mock the network in a UI test?

## Say it in the interview

- "I use role and label locators and web-first assertions. No fixed sleeps."
- "Each test owns its data: unique names, setup through the API, so tests can run in parallel."
- "For a big suite: page objects or fixtures, tags for smoke and regression, sharding in CI, and traces on failure."
- "A test that passes on retry is a bug. I quarantine it and fix the cause."
