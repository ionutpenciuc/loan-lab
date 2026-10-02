import type { CreateLoanInput, LoanAccount, LoanSchedule, ScheduleInput } from './types'

const apiBase = import.meta.env.VITE_API_BASE ?? 'http://localhost:5080'

export function listLoanAccounts(): Promise<LoanAccount[]> {
  return request<LoanAccount[]>('/api/loan-accounts')
}

export function previewSchedule(input: ScheduleInput): Promise<LoanSchedule> {
  return request<LoanSchedule>('/api/loan-accounts/preview', {
    method: 'POST',
    body: JSON.stringify(input),
  })
}

export function createLoanAccount(input: CreateLoanInput): Promise<LoanAccount> {
  return request<LoanAccount>('/api/loan-accounts', {
    method: 'POST',
    body: JSON.stringify(input),
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
