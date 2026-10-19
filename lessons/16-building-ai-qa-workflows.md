# 16 — Building AI QA workflows: hooks, skills, MCP, headless runs and evals

Time: 2 hours. Needs Claude Code (see [tools/claude-code.md](../tools/claude-code.md)).

Goal: go from *using* an AI agent (lesson 08) to *building* the workflows a QA community reuses, and measuring whether they work.

The job ad: "build automation skills and workflows the wider QA community can reuse", and "AI pipelines, harnesses and context engineering are a strong plus". This lesson gives you real files to show for each of those words.

## Learn: the building blocks

| Block | Where | When it runs | QA example in this repo |
| --- | --- | --- | --- |
| Context (`CLAUDE.md`) | repo root | Every session | Commands, conventions, domain rules |
| Skill | `.claude/skills/<name>/SKILL.md` (+ scripts) | When a task matches its `description` | `write-unit-test`, `triage-test-failure`, `find-flaky-tests` (you finish it) |
| Subagent | `.claude/agents/<name>.md` | When delegated; own context and tools | `test-reviewer` |
| Hook | `.claude/settings.json` → `hooks` | On an event, **always**, by the harness, not the model | `.claude/hooks/run-unit-tests.sh` |
| MCP server | `.mcp.json` | Adds tools: browser, Jira, database | Playwright MCP: `.claude/examples/mcp.example.json` |
| Headless run | `claude -p "..."` | From a script or CI job | `ai-triage` job in `ci/gitlab-ci.example.yml` |

**Skill vs hook.** A skill is advice the model chooses to follow. A hook is a guarantee: the harness runs it every time. "Always run tests after editing a test" belongs in a hook, not in `CLAUDE.md`.

### Hooks

Events include `PreToolUse` (can block a tool call), `PostToolUse`, `UserPromptSubmit`, `Stop`. The hook gets JSON on stdin (tool name, file path). Exit code `0` = fine; exit code `2` = blocking error, and stderr goes back to the agent so it must fix the problem.

QA uses: run tests after edits; block edits to `backend/src/` or `instructor/` (`PreToolUse`); block `git push` when tests are red; log every AI change for an audit trail (useful in regulated work).

### Context engineering

Context is a budget. Every token in it competes for the model's attention.

- Put **stable facts** in `CLAUDE.md` (short). Put **procedures** in skills (loaded only when needed). Put **checks** in hooks and tests (not in words).
- Give **examples** of good output. One good test is worth a page of rules.
- Give **rules and expected values**, not "test this code" (or it copies the bugs).
- Use subagents for big reads (logs, traces) so the main context stays clean.
- Keep secrets and patient data out. Synthetic data only.

### An AI harness and evals

How do you know a skill works? The same way you know a test works: **measure it against known bugs.** This repo already has a perfect eval set: the bug kit.

```text
for each bug in the kit:
    turn the bug on
    run the AI-written tests
    caught = at least one test fails
score = caught bugs ÷ all bugs      (and: all tests must pass with no bug on)
```

Change the skill, run the eval again, compare scores. This is an **eval harness**: the same idea as mutation testing, applied to AI output. It turns "the AI seems good" into a number you can show a manager.

## In this repo

| File | What |
| --- | --- |
| `.claude/hooks/run-unit-tests.sh` | A `PostToolUse` hook. Read it. |
| `.claude/examples/settings.hooks.example.json` | Turns the hook on |
| `.claude/examples/mcp.example.json` | Playwright MCP |
| `.claude/skills/find-flaky-tests/SKILL.md` | A skill stub: exercise L16-2 |
| `ci/gitlab-ci.example.yml`, job `ai-triage` | A headless agent in CI |

## Do

1. **L16-1: a hook.** Copy `.claude/examples/settings.hooks.example.json` to `.claude/settings.local.json` (local = not committed). Start `claude` in the repo. Ask it to add one boundary test to `TaperPlanServiceShould.cs` with a **wrong** expected value on purpose. Watch: the hook runs the tests and the agent sees the failure. Write in `notes/ai.md`: why is this better than a line in `CLAUDE.md` that says "run the tests"?
2. **L16-2: finish a skill.** Complete `.claude/skills/find-flaky-tests/SKILL.md`. Remove the `Skip` on `FlakyExamplesShould.ListOnlyTheTwoSeededPlans` (lesson 11). Ask Claude: "Find flaky tests in the API tests." Did it load your skill? Did it find the cause? Improve the skill once, try again. Put the skip back.
3. **L16-3: a guard hook.** Write a second hook, `PreToolUse` on `Edit|Write`, that **blocks** any edit under `backend/src/`, `frontend/src/` or `instructor/` (exit 2 with a clear message). Test it by asking the agent to "fix" product code.
4. **L16-4: headless triage.** Break one test on purpose. Run:
   ```bash
   cd backend && dotnet test --logger "junit;LogFileName={assembly}.junit.xml" --results-directory ../TestResults
   cd .. && claude -p "Use the triage-test-failure skill. Read the JUnit files in TestResults/. Classify each failure. Do not change files." --allowedTools "Read,Grep,Glob"
   ```
   Is the classification right? Then read the `ai-triage` job in `ci/gitlab-ci.example.yml`. Why does it run only `on_failure`, and why only read-only tools?
5. **L16-5: Playwright MCP.** Copy `.claude/examples/mcp.example.json` to `.mcp.json` at the repo root (do not commit it). Start the app. Ask Claude: "Open http://localhost:5173 with the Playwright tools, explore the add-plan form, and list the test cases you would write. Then write one Playwright test in frontend/e2e using role and label locators." Review it with the `test-reviewer` agent.
6. **L16-6: an eval.** Ask the agent, in a fresh session, using the `write-unit-test` skill: "Write unit tests for the dose and week validation rules in TaperPlanService." Save the tests in a new file. Then for bugs `01` and `02`: `instructor/bug.sh on <id>`, run only the new file, note caught or missed, `instructor/bug.sh off <id>`. Also run them with no bug on: they must pass. Write the score in `notes/ai.md`. Change one line of the skill to fix a miss, start a new session, run the eval again. Did the score change?
7. **Show it.** Prepare a 2-minute demo for the interview: the hook catching a failure, your skill, and your eval score.

## Check yourself

1. Skill vs hook vs subagent: when do you use each?
2. What does exit code 2 mean for a hook?
3. What goes in `CLAUDE.md`, and what does not?
4. How would you measure whether an AI test-writing skill is good?
5. Why should an AI job in CI have read-only tools?
6. Name two ways to stop an agent from changing product code.

## Say it in the interview

- "I build QA workflows in layers: a short CLAUDE.md for facts, skills for procedures like writing tests or triage, hooks for guarantees like running tests after every edit, and a reviewer subagent."
- "I measure AI output like any test tool: I run the AI-written tests against seeded bugs and report a catch rate, and they must pass on correct code. When I change a skill, I rerun the eval."
- "In CI, a headless agent triages failures read-only and drafts bug reports. A human decides. That fits regulated work: traceable, reviewable, no patient data."
