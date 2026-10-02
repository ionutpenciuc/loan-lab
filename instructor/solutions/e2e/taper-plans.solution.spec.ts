// Answer key. Copy into frontend/e2e/ to run it.
import { expect, test, type Page } from '@playwright/test'

class TaperPlansPage {
  constructor(private readonly page: Page) {}

  readonly plans = () => this.page.getByRole('table', { name: 'Taper plans' })
  readonly schedule = () => this.page.getByRole('table', { name: 'Taper schedule' })
  readonly error = () => this.page.getByRole('alert')

  async open() {
    await this.page.goto('/')
    await expect(this.plans()).toBeVisible()
  }

  async fillForm(input: { patient: string; medicationCode: string; dose: string; weeks: string }) {
    await this.page.getByRole('button', { name: 'Add taper plan' }).click()
    await this.page.getByLabel('Patient name').fill(input.patient)
    await this.page.getByLabel('Medication').selectOption(input.medicationCode)
    await this.page.getByLabel('Starting daily dose (mg)').fill(input.dose)
    await this.page.getByLabel('Number of weeks').fill(input.weeks)
  }

  async preview() {
    await this.page.getByRole('button', { name: 'Preview schedule' }).click()
  }

  async save() {
    await this.page.getByRole('button', { name: 'Save' }).click()
  }
}

// Catches bug 06. 10 mg over 3 weeks does not divide evenly.
test('previews an uneven taper with two decimals', async ({ page }) => {
  // setup

  const taper = new TaperPlansPage(page)
  await taper.open()
  await taper.fillForm({ patient: 'Preview Only', medicationCode: 'CLM', dose: '10', weeks: '3' })

  // execute

  await taper.preview()

  // verify

  const rows = taper.schedule().locator('tbody tr')
  await expect(rows).toHaveCount(3)
  await expect(rows.nth(1).getByRole('cell').nth(1)).toHaveText('6.67')
  await expect(rows.nth(2).getByRole('cell').nth(1)).toHaveText('3.34')
})

// Catches bug 05. A unique name keeps the test independent of earlier runs.
test('shows a saved plan in the list without a reload', async ({ page }) => {
  // setup

  const patient = `Elena Radu ${Date.now()}`
  const taper = new TaperPlansPage(page)
  await taper.open()
  await taper.fillForm({ patient, medicationCode: 'STR', dose: '40', weeks: '4' })

  // execute

  await taper.save()

  // verify

  await expect(taper.plans().getByRole('cell', { name: patient })).toBeVisible()
})

test('shows the server message when the dose is above the maximum', async ({ page }) => {
  // setup

  const taper = new TaperPlansPage(page)
  await taper.open()
  await taper.fillForm({ patient: 'Too High', medicationCode: 'NRV', dose: '25', weeks: '4' })

  // execute

  await taper.preview()

  // verify

  await expect(taper.error()).toHaveText('Starting daily dose must not exceed 20 mg for Nervalin.')
  await expect(taper.schedule()).toHaveCount(0)
})
