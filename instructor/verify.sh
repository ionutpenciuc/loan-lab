#!/usr/bin/env bash
# Proves the bug kit still works after you change the app.
# For every bug: the sample tests still pass, and the answer-key test for that layer fails.
# Needs: .NET 10 SDK, Node.js, Playwright Chromium, k6. Takes about 5 minutes.
#
#   instructor/verify.sh            check every bug
#   instructor/verify.sh 03 07      check only bugs 03 and 07

set -uo pipefail

root="$(git rev-parse --show-toplevel)"
cd "$root"

api_port=5191
ui_port=5192
api_url="http://localhost:$api_port"
solutions="$root/instructor/solutions"
unit_dir="$root/backend/tests/DoseLab.UnitTests"
api_dir="$root/backend/tests/DoseLab.ApiTests"
e2e_dir="$root/frontend/e2e"
log="$root/instructor/verify.log"
failures=0
api_pid=""

: > "$log"

cleanup() {
  [ -n "$api_pid" ] && kill "$api_pid" 2>/dev/null
  rm -f "$unit_dir"/*SolutionShould.cs "$api_dir"/*SolutionShould.cs "$e2e_dir"/*.solution.spec.ts
  instructor/bug.sh reset >/dev/null
}
trap cleanup EXIT

expect() {
  local label="$1" want="$2"
  shift 2
  if "$@" >>"$log" 2>&1; then got=pass; else got=fail; fi
  if [ "$got" = "$want" ]; then
    echo "  ok    $label ($got)"
  else
    echo "  WRONG $label: expected $want, got $got (see instructor/verify.log)"
    failures=$((failures + 1))
  fi
}

backend_samples() { dotnet test backend/DoseLab.slnx --nologo --filter "FullyQualifiedName!~Solution"; }
backend_solutions() { dotnet test backend/DoseLab.slnx --nologo --filter "FullyQualifiedName~Solution"; }
e2e() { (cd frontend && LAB_API_PORT=$api_port LAB_UI_PORT=$ui_port npx playwright test "$1"); }

k6_run() {
  (cd backend && dotnet run --project src/DoseLab.Api --no-launch-profile --urls "$api_url" >>"$log" 2>&1) &
  api_pid=$!
  for _ in $(seq 1 90); do curl -fs "$api_url/api/medications" >/dev/null 2>&1 && break; sleep 1; done
  k6 run -q -e API_URL="$api_url" "$1"
  local status=$?
  pkill -f "urls $api_url" 2>/dev/null
  wait "$api_pid" 2>/dev/null
  api_pid=""
  return $status
}

check_bug() {
  local id="$1"
  echo "bug $id"
  instructor/bug.sh on "$id" >/dev/null
  case "$id" in
    01|02|03|04)
      expect "sample tests" pass backend_samples
      expect "answer key" fail backend_solutions
      ;;
    05|06)
      expect "sample Playwright test" pass e2e taper-plans.spec.ts
      expect "answer key" fail e2e taper-plans.solution.spec.ts
      ;;
    07)
      expect "smoke test" pass k6_run perf/smoke.js
      expect "answer key" fail k6_run instructor/solutions/perf/list-latency.js
      ;;
    08)
      expect "smoke test" pass k6_run perf/smoke.js
      expect "answer key" fail k6_run instructor/solutions/perf/stress-create.js
      ;;
  esac
  instructor/bug.sh off "$id" >/dev/null
}

instructor/bug.sh reset >/dev/null
cp "$solutions"/backend/TaperScheduleCalculatorSolutionShould.cs "$solutions"/backend/TaperPlanServiceSolutionShould.cs "$unit_dir"/
cp "$solutions"/backend/TaperPlansApiSolutionShould.cs "$api_dir"/
cp "$solutions"/e2e/*.solution.spec.ts "$e2e_dir"/

echo "no bugs"
expect "backend samples and answer keys" pass dotnet test backend/DoseLab.slnx --nologo
expect "Playwright samples and answer keys" pass e2e ""
expect "k6 list latency" pass k6_run instructor/solutions/perf/list-latency.js
expect "k6 stress create" pass k6_run instructor/solutions/perf/stress-create.js

if [ $# -gt 0 ]; then ids=("$@"); else ids=(01 02 03 04 05 06 07 08); fi
for id in "${ids[@]}"; do check_bug "$id"; done

echo
if [ "$failures" -eq 0 ]; then echo "Bug kit OK."; else echo "$failures check(s) went wrong."; exit 1; fi
