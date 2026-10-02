import { expect, test } from '@playwright/test'

test('shows the seeded loan accounts', async ({ page }) => {
  // setup

  // execute

  await page.goto('/')

  // verify

  await expect(page.getByRole('heading', { name: 'Loan accounts' })).toBeVisible()
  await expect(page.getByRole('cell', { name: 'Maria Ionescu' })).toBeVisible()
  await expect(page.getByRole('cell', { name: 'Andrei Stan' })).toBeVisible()
})

// Add your own tests below this line.
