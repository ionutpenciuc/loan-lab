import { expect, test } from '@playwright/test'

test('shows the seeded taper plans', async ({ page }) => {
  // setup

  // execute

  await page.goto('/')

  // verify

  await expect(page.getByRole('heading', { name: 'Taper plans' })).toBeVisible()
  const plans = page.getByRole('table', { name: 'Taper plans' })
  await expect(plans.getByRole('cell', { name: 'Maria Ionescu' })).toBeVisible()
  await expect(plans.getByRole('cell', { name: 'Andrei Stan' })).toBeVisible()
})

// Add your own tests below this line.
