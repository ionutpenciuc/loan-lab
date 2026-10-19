# Claude Code

Needed for lessons 08 and 16.

## Install on Windows

Git for Windows must be installed first ([windows-setup.md](windows-setup.md)). In PowerShell:

```powershell
irm https://claude.ai/install.ps1 | iex
```

Or with Node.js: `npm install -g @anthropic-ai/claude-code`.

Close and reopen the terminal. Then, in the repo folder:

```powershell
claude
```

The first start opens a browser to log in. You need a Claude subscription (Pro or higher) or an Anthropic API account. Docs: https://docs.claude.com/en/docs/claude-code/overview

## First steps in this repo

- `/init` is not needed: `CLAUDE.md` already exists. Read it.
- Type `/` to see commands. `/agents` lists `test-reviewer`. Skills in `.claude/skills/` load by themselves when a request matches.
- `/permissions` shows what the agent may do without asking.
- `Esc` stops the agent. `Esc Esc` goes back to an earlier message.

## Files you will create (not committed)

| File | What |
| --- | --- |
| `.claude/settings.local.json` | Your hooks (lesson 16). `.local` = only on your machine. |
| `.mcp.json` | MCP servers, for example Playwright MCP (lesson 16) |

## Rules for this lab

- Synthetic data only. Never paste real patient data into any AI tool.
- The agent must not read `instructor/` (this is in `CLAUDE.md`). Lesson 16 shows how to enforce it with a hook.
- You own every test the agent writes. Review it, run it, and see it fail once.

Similar tools work the same way: Cursor (rules, agents, MCP), GitHub Copilot (instructions file, agent mode, MCP).
