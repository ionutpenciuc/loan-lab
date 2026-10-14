# Tools: install and first use

Guides for every external tool in the lab, written for **Windows 10/11**. Install what each lesson needs, when you get to it.

| Guide | Tools | Needed for |
| --- | --- | --- |
| [windows-setup.md](windows-setup.md) | Git, .NET 10 SDK, Node.js, an IDE, k6 | Everything (day 1) |
| [docker-and-postgres.md](docker-and-postgres.md) | Docker Desktop, PostgreSQL in a container | Lessons 12, 14 |
| [stryker-and-coverage.md](stryker-and-coverage.md) | Coverlet, ReportGenerator, Stryker.NET | Lesson 11 |
| [dotnet-diagnostics.md](dotnet-diagnostics.md) | dotnet-counters, dotnet-trace, BenchmarkDotNet | Lesson 13 |
| [zap-and-burp.md](zap-and-burp.md) | OWASP ZAP, Burp Suite Community | Lessons 07, 14 |
| [flaui-and-desktop.md](flaui-and-desktop.md) | FlaUI, Accessibility Insights, FlaUInspect | Lesson 15 |
| [ranorex.md](ranorex.md) | Ranorex Studio (trial) | Lesson 15, optional |
| [cypress.md](cypress.md) | Cypress | Lesson 15, optional |
| [gitlab-ci.md](gitlab-ci.md) | GitLab CI | Lesson 06, optional |
| [claude-code.md](claude-code.md) | Claude Code | Lessons 08, 16 |

**winget** is the Windows package manager (built into Windows 11 and recent Windows 10). Commands below use it. If a command fails, use the download link instead.

After installing anything: **close and reopen the terminal**, so it sees the new `PATH`.
