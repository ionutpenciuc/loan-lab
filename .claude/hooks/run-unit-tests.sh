#!/usr/bin/env bash
# PostToolUse hook (lesson 16). After Claude Code edits a C# file under backend/tests/DoseLab.UnitTests,
# run the unit tests. If they fail, exit 2: Claude Code shows stderr to the agent, so it must react.
# Turn it on by copying .claude/examples/settings.hooks.example.json to .claude/settings.local.json.

input=$(cat)
file=$(printf '%s' "$input" | node -e '
  let data = ""
  process.stdin.on("data", (chunk) => (data += chunk)).on("end", () => {
    try { console.log(JSON.parse(data).tool_input?.file_path ?? "") } catch { console.log("") }
  })')
file="${file//\\//}"   # Windows paths use backslashes

case "$file" in
  */backend/tests/DoseLab.UnitTests/*.cs) ;;
  *) exit 0 ;;
esac

cd "${CLAUDE_PROJECT_DIR:-.}/backend" || exit 0
if ! output=$(dotnet test tests/DoseLab.UnitTests --nologo 2>&1); then
  {
    echo "Unit tests fail after your edit of $file:"
    printf '%s\n' "$output" | grep -vE "\[SKIP\]|Skipped " | grep -E "\[FAIL\]|Assert|Expected|Actual|error CS" | head -40
  } >&2
  exit 2
fi
exit 0
