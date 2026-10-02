import { useEffect, useState, type FormEvent } from 'react'
import { createLoanAccount, listLoanAccounts, previewSchedule } from './api'
import type { LoanAccount, LoanSchedule } from './types'

export default function App() {
  const [accounts, setAccounts] = useState<LoanAccount[]>([])
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [formOpen, setFormOpen] = useState(false)
  const [customerName, setCustomerName] = useState('')
  const [amount, setAmount] = useState('')
  const [annualInterestRate, setAnnualInterestRate] = useState('')
  const [installmentCount, setInstallmentCount] = useState('')
  const [schedule, setSchedule] = useState<LoanSchedule | null>(null)
  const [formError, setFormError] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    void loadAccounts()
  }, [])

  async function loadAccounts() {
    setLoading(true)
    setLoadError(null)
    try {
      setAccounts(await listLoanAccounts())
    } catch {
      setLoadError('Could not load loan accounts. Start the API at http://localhost:5080.')
    } finally {
      setLoading(false)
    }
  }

  function readScheduleInput():
    | { amount: number; annualInterestRate: number; installmentCount: number }
    | string {
    if (!/^\d+$/.test(installmentCount.trim())) {
      return 'Number of installments must be a whole number from 1 to 360.'
    }

    const parsedAmount = Number(amount)
    const parsedRate = Number(annualInterestRate)
    if (!Number.isFinite(parsedAmount) || !Number.isFinite(parsedRate)) {
      return 'Enter a loan amount and an annual interest rate.'
    }

    return {
      amount: parsedAmount,
      annualInterestRate: parsedRate,
      installmentCount: Number(installmentCount),
    }
  }

  async function onPreview() {
    const input = readScheduleInput()
    if (typeof input === 'string') {
      setSchedule(null)
      setFormError(input)
      return
    }

    setBusy(true)
    setFormError(null)
    try {
      setSchedule(await previewSchedule(input))
    } catch (error) {
      setSchedule(null)
      setFormError(error instanceof Error ? error.message : 'Could not preview the schedule.')
    } finally {
      setBusy(false)
    }
  }

  async function onSave(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const input = readScheduleInput()
    if (typeof input === 'string') {
      setFormError(input)
      return
    }

    setBusy(true)
    setFormError(null)
    try {
      await createLoanAccount({ ...input, customerName })
      setCustomerName('')
      setAmount('')
      setAnnualInterestRate('')
      setInstallmentCount('')
      setSchedule(null)
      setFormOpen(false)
      await loadAccounts()
    } catch (error) {
      setFormError(error instanceof Error ? error.message : 'Could not save the loan account.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <main>
      <header className="page-header">
        <div>
          <h1>Loan accounts</h1>
          <p>DBE schedule: equal principal each month. Interest is charged on the remaining balance.</p>
        </div>
        <button type="button" onClick={() => setFormOpen((open) => !open)} aria-expanded={formOpen}>
          Add loan account
        </button>
      </header>

      {loading && <p>Loading loan accounts…</p>}
      {loadError && <p role="alert">{loadError}</p>}

      {!loading && !loadError && (
        <table aria-label="Loan accounts">
          <thead>
            <tr>
              <th scope="col">Customer</th>
              <th scope="col">Amount</th>
              <th scope="col">Interest rate</th>
              <th scope="col">Installments</th>
              <th scope="col">Created</th>
            </tr>
          </thead>
          <tbody>
            {accounts.map((account) => (
              <tr key={account.id}>
                <td>{account.customerName}</td>
                <td className="num">{formatMoney(account.amount)}</td>
                <td className="num">{account.annualInterestRate}%</td>
                <td className="num">{account.installmentCount}</td>
                <td>{account.createdOn}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {formOpen && (
        <section className="panel">
          <h2>New loan account</h2>
          <form onSubmit={onSave}>
            <label htmlFor="customer-name">Customer name</label>
            <input
              id="customer-name"
              value={customerName}
              onChange={(event) => setCustomerName(event.target.value)}
              autoFocus
              required
            />

            <label htmlFor="loan-amount">Loan amount</label>
            <input
              id="loan-amount"
              value={amount}
              onChange={(event) => setAmount(event.target.value)}
              inputMode="decimal"
              required
            />

            <label htmlFor="interest-rate">Annual interest rate</label>
            <input
              id="interest-rate"
              value={annualInterestRate}
              onChange={(event) => setAnnualInterestRate(event.target.value)}
              inputMode="decimal"
              required
            />
            <p className="hint">Percent per year. Example: 12</p>

            <label htmlFor="installment-count">Number of installments</label>
            <input
              id="installment-count"
              value={installmentCount}
              onChange={(event) => setInstallmentCount(event.target.value)}
              inputMode="numeric"
              required
            />
            <p className="hint">Whole number of months, from 1 to 360.</p>

            {formError && <p role="alert">{formError}</p>}

            <div className="actions">
              <button type="button" onClick={() => void onPreview()} disabled={busy}>
                Preview schedule
              </button>
              <button type="submit" disabled={busy}>
                Save
              </button>
            </div>
          </form>

          {schedule && (
            <>
              <h2>Repayment schedule</h2>
              <table aria-label="Repayment schedule">
                <thead>
                  <tr>
                    <th scope="col">#</th>
                    <th scope="col">Principal</th>
                    <th scope="col">Interest</th>
                    <th scope="col">Installment</th>
                    <th scope="col">Remaining balance</th>
                  </tr>
                </thead>
                <tbody>
                  {schedule.lines.map((line) => (
                    <tr key={line.number}>
                      <td className="num">{line.number}</td>
                      <td className="num">{formatMoney(line.principal)}</td>
                      <td className="num">{formatMoney(line.interest)}</td>
                      <td className="num">{formatMoney(line.installment)}</td>
                      <td className="num">{formatMoney(line.remainingBalance)}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr>
                    <th scope="row">Total</th>
                    <td className="num">{formatMoney(schedule.totalPrincipal)}</td>
                    <td className="num">{formatMoney(schedule.totalInterest)}</td>
                    <td className="num">{formatMoney(schedule.totalInstallment)}</td>
                    <td></td>
                  </tr>
                </tfoot>
              </table>
            </>
          )}
        </section>
      )}
    </main>
  )
}

function formatMoney(value: number): string {
  return value.toLocaleString('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })
}
