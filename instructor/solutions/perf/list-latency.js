// Answer key for bug 07 (list slows down as plans grow).
// Setup creates many plans, then 10 users read the list.
// Run: k6 run instructor/solutions/perf/list-latency.js

import http from 'k6/http'
import { check } from 'k6'

const API_URL = __ENV.API_URL || 'http://localhost:5080'
const API_KEY = __ENV.API_KEY || 'lab-dev-key'
const PLANS_TO_CREATE = Number(__ENV.PLANS || 200)

export const options = {
  setupTimeout: '120s',
  scenarios: {
    read_list: {
      executor: 'constant-vus',
      vus: 10,
      duration: '15s',
    },
  },
  thresholds: {
    http_req_failed: ['rate<0.01'],
    'http_req_duration{name:list}': ['p(95)<250'],
  },
}

export function setup() {
  const headers = { 'Content-Type': 'application/json', 'X-Api-Key': API_KEY }
  for (let i = 0; i < PLANS_TO_CREATE; i++) {
    const body = JSON.stringify({
      patientName: `Load patient ${i}`,
      medicationCode: 'STR',
      startingDailyDoseMg: 40,
      weekCount: 4,
    })
    http.post(`${API_URL}/api/taper-plans`, body, { headers, tags: { name: 'setup-create' } })
  }
}

export default function () {
  const response = http.get(`${API_URL}/api/taper-plans`, { tags: { name: 'list' } })
  check(response, { 'status is 200': (r) => r.status === 200 })
}
