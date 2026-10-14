# Cypress (optional)

Lesson 15 compares Cypress with Playwright. This is a 15-minute try-out on the lab UI. The repo does not keep Cypress (the suite is Playwright), so nothing here is committed.

## Try it

Start the API and the UI first. Then from `frontend`:

```powershell
npm install --no-save cypress @testing-library/cypress
```

Create `frontend/cypress.config.ts`:

```ts
import { defineConfig } from 'cypress'

export default defineConfig({
  e2e: { baseUrl: 'http://localhost:5173', supportFile: 'cypress/support/e2e.ts' },
})
```

Create `frontend/cypress/support/e2e.ts`:

```ts
import '@testing-library/cypress/add-commands'
```

Create `frontend/cypress/e2e/taper-plans.cy.ts`:

```ts
describe('taper plans', () => {
  it('shows the seeded plans', () => {
    cy.visit('/')
    cy.findByRole('cell', { name: 'Maria Ionescu' }).should('be.visible')
  })

  it('previews an uneven taper', () => {
    cy.visit('/')
    cy.findByRole('button', { name: 'Add taper plan' }).click()
    // TODO: fill the form like your Playwright test, click Preview, and check week 2 shows 6.67
  })
})
```

Run:

```powershell
npx cypress open      # interactive runner: pick E2E, Chrome, the spec
npx cypress run       # headless
```

Then delete the three files and the `cypress` folder. `npm ci` removes the packages.

## Notice

- Commands chain and retry: `cy.findByRole(...).should(...)`. No `await`.
- The runner's **time travel**: hover over each command to see the page at that moment.
- `cy.intercept()` is the same idea as Playwright's `page.route()`.

Docs: https://docs.cypress.io
