export interface LoanAccount {
  id: string
  customerName: string
  amount: number
  annualInterestRate: number
  installmentCount: number
  createdOn: string
}

export interface ScheduleLine {
  number: number
  principal: number
  interest: number
  installment: number
  remainingBalance: number
}

export interface LoanSchedule {
  lines: ScheduleLine[]
  totalPrincipal: number
  totalInterest: number
  totalInstallment: number
}

export interface ScheduleInput {
  amount: number
  annualInterestRate: number
  installmentCount: number
}

export interface CreateLoanInput extends ScheduleInput {
  customerName: string
}
