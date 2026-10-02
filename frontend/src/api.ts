import type { CreateTaperPlanInput, Medication, ScheduleInput, TaperPlan, TaperSchedule } from './types'

const apiBase = import.meta.env.VITE_API_BASE ?? 'http://localhost:5080'
const apiKey = import.meta.env.VITE_API_KEY ?? 'lab-dev-key'

export function listMedications(): Promise<Medication[]> {
  return request<Medication[]>('/api/medications')
}

export function listTaperPlans(): Promise<TaperPlan[]> {
  return request<TaperPlan[]>('/api/taper-plans')
}

export function previewSchedule(input: ScheduleInput): Promise<TaperSchedule> {
  return request<TaperSchedule>('/api/taper-plans/preview', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

export function createTaperPlan(input: CreateTaperPlanInput): Promise<TaperPlan> {
  return request<TaperPlan>('/api/taper-plans', {
    method: 'POST',
    body: JSON.stringify(input),
    headers: { 'X-Api-Key': apiKey },
  })
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBase}${path}`, {
    ...init,
    headers: {
      'Content-Type': 'application/json',
      ...(init?.headers ?? {}),
    },
  })

  if (!response.ok) {
    const body = (await response.json().catch(() => null)) as { message?: string } | null
    throw new Error(body?.message ?? `Request failed (${response.status})`)
  }

  return response.json() as Promise<T>
}
