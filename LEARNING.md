# 3-day plan — testing pyramid

You have about 4 hours each day. You do not need to learn all of C# or React.

Goal: explain the testing pyramid with this app, and add a few tests yourself.

## The rule this app uses

DBE means equal principal each month.

Interest for a month = remaining balance × annual rate / 100 / 12.

Round money to 2 decimals, half away from zero.

The last principal is the remainder. The principal column then sums to the loan amount.

Example already covered by a test:

- Amount `1200`
- Annual rate `12` (1% per month)
- `12` installments
- Principal each month: `100`
- Month 1 interest: `12`. Installment: `112`. Remaining: `1100`
- Month 12 interest: `1`. Installment: `101`. Remaining: `0`
- Total interest: `78`

## The pyramid in this repo

```text
          Playwright          few tests. the real screen
        API tests             HTTP, no browser
      service tests           save rules, no HTTP
    calculator tests          many tests. the money math
```

| Layer | Sample file | What it proves |
| --- | --- | --- |
| Calculator | `backend/tests/LoanLab.UnitTests/DbeScheduleCalculatorShould.cs` | The schedule numbers. No database. No HTTP. No browser. |
| Service | `backend/tests/LoanLab.UnitTests/LoanAccountServiceShould.cs` | A loan is saved in the in-memory list. |
| API | `backend/tests/LoanLab.ApiTests/LoanAccountsApiShould.cs` | `GET /api/loan-accounts` returns the two seeded loans. |
| Browser | `frontend/e2e/loan-accounts.spec.ts` | The page shows Maria Ionescu and Andrei Stan. |

Read the code in this order:

1. `backend/src/LoanLab.Application/DbeScheduleCalculator.cs`
2. `backend/tests/LoanLab.UnitTests/DbeScheduleCalculatorShould.cs`
3. `backend/src/LoanLab.Application/LoanAccountService.cs`
4. `backend/tests/LoanLab.UnitTests/LoanAccountServiceShould.cs`
5. `backend/src/LoanLab.Api/Controllers/LoanAccountsController.cs`
6. `backend/tests/LoanLab.ApiTests/LoanAccountsApiShould.cs`
7. `frontend/src/App.tsx`
8. `frontend/e2e/loan-accounts.spec.ts`

The controller is thin. It calls the service and returns JSON. This repo does not unit-test the controller. The API test covers HTTP.

## Day 1 — Run the app and map the tests

1. Follow the README until the app is open and the sample loan `Ana Pop` is in the list.
2. Run `dotnet test` from `backend`. 3 tests pass.
3. Run `npm run test:e2e` from `frontend`. 1 test passes.
4. Read the 8 files above. For each sample test, write one sentence:
   - What bug would this test catch?
   - What bug would this test miss?

Keep those sentences. You will say them in the interview.

## Day 2 — Add two unit tests

Work only in the backend test project. Copy the style of the sample tests.

### Test A — zero interest

Open `DbeScheduleCalculatorShould.cs`. Paste this under the comment at the bottom.

```csharp
[Fact]
public void ChargeNoInterestWhenTheRateIsZero()
{
    // setup

    var amount = 900m;
    var annualInterestRate = 0m;
    var installmentCount = 3;

    // execute

    var schedule = DbeScheduleCalculator.Calculate(amount, annualInterestRate, installmentCount);

    // verify

    Assert.Equal(3, schedule.Lines.Count);
    Assert.Equal(300m, schedule.Lines[0].Principal);
    Assert.Equal(0m, schedule.Lines[0].Interest);
    Assert.Equal(300m, schedule.Lines[0].Installment);
    Assert.Equal(600m, schedule.Lines[0].RemainingBalance);
    Assert.Equal(0m, schedule.Lines[2].RemainingBalance);
    Assert.Equal(900m, schedule.TotalPrincipal);
    Assert.Equal(0m, schedule.TotalInterest);
}
```

From `backend` run:

```bash
dotnet test
```

The new test must pass.

Then change `0m` interest expectation to `1m`. Run `dotnet test` again. The test must fail. Read the failure. It shows expected `1` and actual `0`. Put `0m` back. Run the tests. Green again.

That is a red/green cycle. Say this in the interview: a good test fails when the behavior is wrong.

### Test B — empty name is rejected

Open `LoanAccountServiceShould.cs`. Paste this under the comment.

```csharp
[Fact]
public void RejectAnEmptyCustomerName()
{
    // setup

    var repository = new InMemoryLoanAccountRepository();
    var service = new LoanAccountService(repository);

    // execute

    var error = Assert.Throws<LoanValidationException>(() =>
        service.Create("   ", 1200m, 12m, 12));

    // verify

    Assert.Equal("Customer name is required.", error.Message);
    Assert.Empty(service.List());
}
```

Run `dotnet test`. It must pass.

This test uses a new empty repository. It does not call HTTP. Preview and save rules belong here, not in the browser.

## Day 3 — Add one browser test

Open `frontend/e2e/loan-accounts.spec.ts`. Paste this under the comment.

```ts
test('previews a 12 month DBE schedule', async ({ page }) => {
  // setup

  await page.goto('/')

  // execute

  await page.getByRole('button', { name: 'Add loan account' }).click()
  await page.getByLabel('Customer name').fill('Elena Radu')
  await page.getByLabel('Loan amount').fill('1200')
  await page.getByLabel('Annual interest rate').fill('12')
  await page.getByLabel('Number of installments').fill('12')
  await page.getByRole('button', { name: 'Preview schedule' }).click()

  // verify

  const schedule = page.getByRole('table', { name: 'Repayment schedule' })
  await expect(schedule.locator('tbody tr')).toHaveCount(12)
  await expect(schedule.getByRole('cell', { name: '112.00' })).toBeVisible()
})
```

From `frontend` run:

```bash
npm run test:e2e
```

Both browser tests must pass.

`toHaveCount(12)` counts body rows only. The header row is not a data row. Say that if someone asks why the count is not 13.

The test does not use `sleep`. `expect` waits until the row appears or the timeout hits.

Do not put the zero-interest case in Playwright. That case is already a unit test. It is faster and more exact there.

## Stretch, if time remains

Calculator. Amount `100`, rate `0`, `3` installments. Principals are `33.33`, `33.33`, and `33.34`. The last line keeps the remainder.

Service. `Preview` does not add a loan. Call `Preview(1200m, 12m, 12)` and assert the list is empty.

Browser. After the preview test, click `Save` and assert a cell `Elena Radu` is visible.

API. In `LoanAccountsApiShould.cs`, POST a loan and GET the list. Assert the new name is present. Do not assert that the list has exactly 2 rows. The sample loans are already there, and this test adds one more.

## Sentences for the interview

1. The pyramid has many fast tests at the bottom and few slow tests at the top. Here, schedule math is the bottom. The browser is the top.
2. I test rounding in a unit test. A `0.01` error in the last principal does not need a browser.
3. I test "empty name is rejected" on the service, with an in-memory repository. No API. No browser.
4. I use one API test to prove the URL returns the seeded loans. I do not repeat every schedule case over HTTP.
5. I use Playwright for what only the screen can prove: the list renders, preview shows 12 rows, save adds a row.
6. I do not assert today's date. The date changes. The sample loans use fixed dates, `2026-01-15` and `2026-02-01`.
7. A browser test waits on a visible result. It does not sleep for a fixed time.

## Bugs each layer catches

- Calculator: the last principal is short by `0.01`, so the column does not sum to the loan amount.
- Service: a blank name is stored.
- API: the list URL is wrong, or the response is not JSON.
- Playwright: preview is not wired to the button, or save does not refresh the table.
