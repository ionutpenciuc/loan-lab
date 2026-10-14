#!/usr/bin/env bash
# Answer key for L16-3. A PreToolUse hook on Edit|Write that blocks edits to product code and the instructor folder.
# Register it in .claude/settings.local.json under "PreToolUse", like the PostToolUse example.
file=$(node -e '
  let data = ""
  process.stdin.on("data", (chunk) => (data += chunk)).on("end", () => {
    try { console.log(JSON.parse(data).tool_input?.file_path ?? "") } catch { console.log("") }
  })')
file="${file//\\//}"

case "$file" in
  */backend/src/*|*/frontend/src/*|*/desktop/src/*|*/instructor/*)
    echo "Blocked: $file is product code or instructor material. Do not change it to make a test pass. Report a possible bug instead (triage-test-failure skill)." >&2
    exit 2
    ;;
esac
exit 0
