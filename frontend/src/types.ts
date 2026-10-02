export interface Medication {
  code: string
  name: string
  maxDailyDoseMg: number
}

export interface TaperPlan {
  id: string
  referenceNumber: string
  patientName: string
  medicationCode: string
  medicationName: string
  startingDailyDoseMg: number
  weekCount: number
  createdOn: string
}

export interface ScheduleWeek {
  week: number
  dailyDoseMg: number
  weeklyTotalMg: number
  cumulativeTotalMg: number
}

export interface TaperSchedule {
  weeks: ScheduleWeek[]
  totalMg: number
}

export interface ScheduleInput {
  medicationCode: string
  startingDailyDoseMg: number
  weekCount: number
}

export interface CreateTaperPlanInput extends ScheduleInput {
  patientName: string
}
