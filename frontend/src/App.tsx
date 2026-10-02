import { useEffect, useState, type FormEvent } from 'react'
import { createTaperPlan, listMedications, listTaperPlans, previewSchedule } from './api'
import type { Medication, ScheduleInput, TaperPlan, TaperSchedule } from './types'

export default function App() {
  const [plans, setPlans] = useState<TaperPlan[]>([])
  const [medications, setMedications] = useState<Medication[]>([])
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [formOpen, setFormOpen] = useState(false)
  const [patientName, setPatientName] = useState('')
  const [medicationCode, setMedicationCode] = useState('')
  const [startingDose, setStartingDose] = useState('')
  const [weekCount, setWeekCount] = useState('')
  const [schedule, setSchedule] = useState<TaperSchedule | null>(null)
  const [formError, setFormError] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)

  useEffect(() => {
    void loadPlans()
    listMedications()
      .then(setMedications)
      .catch(() => setMedications([]))
  }, [])

  async function loadPlans() {
    setLoading(true)
    setLoadError(null)
    try {
      setPlans(await listTaperPlans())
    } catch {
      setLoadError('Could not load taper plans. Start the API at http://localhost:5080.')
    } finally {
      setLoading(false)
    }
  }

  function readScheduleInput(): ScheduleInput | string {
    if (medicationCode === '') {
      return 'Select a medication.'
    }
    if (!/^\d+$/.test(weekCount.trim())) {
      return 'Number of weeks must be a whole number from 1 to 52.'
    }

    const dose = Number(startingDose)
    if (startingDose.trim() === '' || !Number.isFinite(dose)) {
      return 'Enter a starting daily dose in mg.'
    }

    return {
      medicationCode,
      startingDailyDoseMg: dose,
      weekCount: Number(weekCount),
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
      await createTaperPlan({ ...input, patientName })
      setPatientName('')
      setMedicationCode('')
      setStartingDose('')
      setWeekCount('')
      setSchedule(null)
      setFormOpen(false)
      await loadPlans()
    } catch (error) {
      setFormError(error instanceof Error ? error.message : 'Could not save the taper plan.')
    } finally {
      setBusy(false)
    }
  }

  return (
    <main>
      <header className="page-header">
        <div>
          <h1>Taper plans</h1>
          <p>
            The daily dose drops by the same step every week. Medications and limits are fictional, for practice
            only.
          </p>
        </div>
        <button type="button" onClick={() => setFormOpen((open) => !open)} aria-expanded={formOpen}>
          Add taper plan
        </button>
      </header>

      {loading && <p>Loading taper plans…</p>}
      {loadError && <p role="alert">{loadError}</p>}

      {!loading && !loadError && (
        <table aria-label="Taper plans">
          <thead>
            <tr>
              <th scope="col">Reference</th>
              <th scope="col">Patient</th>
              <th scope="col">Medication</th>
              <th scope="col">Starting dose (mg/day)</th>
              <th scope="col">Weeks</th>
              <th scope="col">Created</th>
            </tr>
          </thead>
          <tbody>
            {plans.map((plan) => (
              <tr key={plan.id}>
                <td>{plan.referenceNumber}</td>
                <td>{plan.patientName}</td>
                <td>{plan.medicationName}</td>
                <td className="num">{formatDose(plan.startingDailyDoseMg)}</td>
                <td className="num">{plan.weekCount}</td>
                <td>{plan.createdOn}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {formOpen && (
        <section className="panel">
          <h2>New taper plan</h2>
          <form onSubmit={onSave}>
            <label htmlFor="patient-name">Patient name</label>
            <input
              id="patient-name"
              value={patientName}
              onChange={(event) => setPatientName(event.target.value)}
              autoFocus
            />

            <label htmlFor="medication">Medication</label>
            <select id="medication" value={medicationCode} onChange={(event) => setMedicationCode(event.target.value)}>
              <option value="">Select…</option>
              {medications.map((medication) => (
                <option key={medication.code} value={medication.code}>
                  {medication.name} (max {formatDose(medication.maxDailyDoseMg)} mg/day)
                </option>
              ))}
            </select>

            <label htmlFor="starting-dose">Starting daily dose (mg)</label>
            <input
              id="starting-dose"
              value={startingDose}
              onChange={(event) => setStartingDose(event.target.value)}
              inputMode="decimal"
            />

            <label htmlFor="week-count">Number of weeks</label>
            <input
              id="week-count"
              value={weekCount}
              onChange={(event) => setWeekCount(event.target.value)}
              inputMode="numeric"
            />
            <p className="hint">Whole number, from 1 to 52.</p>

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
              <h2>Taper schedule</h2>
              <table aria-label="Taper schedule">
                <thead>
                  <tr>
                    <th scope="col">Week</th>
                    <th scope="col">Daily dose (mg)</th>
                    <th scope="col">Weekly total (mg)</th>
                    <th scope="col">Cumulative (mg)</th>
                  </tr>
                </thead>
                <tbody>
                  {schedule.weeks.map((week) => (
                    <tr key={week.week}>
                      <td className="num">{week.week}</td>
                      <td className="num">{formatDose(week.dailyDoseMg)}</td>
                      <td className="num">{formatDose(week.weeklyTotalMg)}</td>
                      <td className="num">{formatDose(week.cumulativeTotalMg)}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr>
                    <th scope="row">Total</th>
                    <td></td>
                    <td></td>
                    <td className="num">{formatDose(schedule.totalMg)}</td>
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

function formatDose(value: number): string {
  return value.toLocaleString('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  })
}
