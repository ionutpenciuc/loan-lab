// Answer key for bug 08 (duplicate reference numbers under concurrent saves).
// Ramps up to 50 users that all create plans, then checks every reference is unique.
// Run: k6 run instructor/solutions/perf/stress-create.js

import http from 'k6/http'
import { check, sleep } from 'k6'
import { Counter } from 'k6/metrics'

const API_URL = __ENV.API_URL || 'http://localhost:5080'
const API_KEY = __ENV.API_KEY || 'lab-dev-key'

const duplicateReferences = new Counter('duplicate_references')

export const options = {
  scenarios: {
    create_plans: {
      executor: 'ramping-vus',
      startVUs: 1,
      stages: [
        { duration: '5s', target: 50 },
        { duration: '10s', target: 50 },
        { duration: '5s', target: 0 },
      ],
    },
  },
  thresholds: {
    http_req_failed: ['rate<0.01'],
    'http_req_duration{name:create}': ['p(95)<500'],
    duplicate_references: ['count==0'],
  },
}

export default function () {
  const body = JSON.stringify({
    patientName: `Stress patient ${__VU}-${__ITER}`,
    medicationCode: 'CLM',
    startingDailyDoseMg: 20,
    weekCount: 5,
  })
  const response = http.post(`${API_URL}/api/taper-plans`, body, {
    headers: { 'Content-Type': 'application/json', 'X-Api-Key': API_KEY },
    tags: { name: 'create' },
  })
  check(response, { 'status is 201': (r) => r.status === 201 })
  sleep(0.1)
}

export function teardown() {
  const plans = http.get(`${API_URL}/api/taper-plans`).json()
  const seen = new Set()
  let duplicates = 0
  for (const plan of plans) {
    if (seen.has(plan.referenceNumber)) duplicates++
    seen.add(plan.referenceNumber)
  }
  duplicateReferences.add(duplicates)
  console.log(`plans=${plans.length} duplicate references=${duplicates}`)
}
