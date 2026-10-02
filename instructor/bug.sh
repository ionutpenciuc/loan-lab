#!/usr/bin/env bash
# Turns the hidden practice bugs on and off.
#
#   instructor/bug.sh list            show every bug and whether it is on
#   instructor/bug.sh on 01 05        turn bugs 01 and 05 on
#   instructor/bug.sh off 05          turn bug 05 off
#   instructor/bug.sh random 3        turn 3 random bugs on (others stay as they are)
#   instructor/bug.sh reset           turn every bug off
#
# Works on macOS, Linux, and Git Bash on Windows. Restart the API and the UI after a change.

set -euo pipefail

root="$(git rev-parse --show-toplevel)"
bugs_dir="$root/instructor/bugs"
cd "$root"

usage() {
  sed -n '2,10p' "$0" | sed 's/^# \{0,1\}//'
  exit 1
}

patch_for() {
  local match="$bugs_dir/$1.patch"
  if [ ! -f "$match" ]; then
    echo "No bug with id '$1'. Run: instructor/bug.sh list" >&2
    exit 1
  fi
  echo "$match"
}

is_on() {
  git apply --reverse --check "$1" >/dev/null 2>&1
}

turn_on() {
  local patch="$1"
  if is_on "$patch"; then
    echo "already on: $(basename "$patch" .patch)"
  else
    git apply "$patch"
    echo "on:  $(basename "$patch" .patch)"
  fi
}

turn_off() {
  local patch="$1"
  if is_on "$patch"; then
    git apply --reverse "$patch"
    echo "off: $(basename "$patch" .patch)"
  fi
}

command="${1:-}"
[ -n "$command" ] || usage
shift

case "$command" in
  list)
    for patch in "$bugs_dir"/*.patch; do
      if is_on "$patch"; then state="ON "; else state="off"; fi
      echo "$state  $(basename "$patch" .patch)"
    done
    ;;
  on)
    [ $# -gt 0 ] || usage
    for id in "$@"; do turn_on "$(patch_for "$id")"; done
    ;;
  off)
    [ $# -gt 0 ] || usage
    for id in "$@"; do turn_off "$(patch_for "$id")"; done
    ;;
  random)
    count="${1:-3}"
    ls "$bugs_dir"/*.patch | awk 'BEGIN { srand() } { print rand() "\t" $0 }' | sort -n | cut -f2 | head -n "$count" |
      while read -r patch; do
        if ! is_on "$patch"; then git apply "$patch"; fi
      done
    echo "$count random bugs are on. Run 'instructor/bug.sh list' to see which (after the exam)."
    ;;
  reset)
    for patch in "$bugs_dir"/*.patch; do turn_off "$patch"; done
    echo "All bugs are off."
    ;;
  *)
    usage
    ;;
esac
