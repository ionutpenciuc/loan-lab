# Code coverage and mutation testing

Needed for lesson 11. All tools are listed in `backend/dotnet-tools.json`, so one command installs them for this repo:

```powershell
cd backend
dotnet tool restore
```

## Code coverage (Coverlet + ReportGenerator)

Coverlet is already a package in the test projects. From the repo root:

```powershell
dotnet test backend/DoseLab.slnx --collect "XPlat Code Coverage" --results-directory TestResults
cd backend
dotnet reportgenerator -reports:"../TestResults/**/coverage.cobertura.xml" -targetdir:../coverage-report -reporttypes:Html
start ../coverage-report/index.html
```

Read: line coverage, branch coverage, then click a file. Red lines never ran. Yellow lines had a branch that never ran.

Visual Studio Enterprise and Rider (dotCover) also show coverage inside the editor.

## Mutation testing (Stryker.NET)

```powershell
cd backend/tests/DoseLab.UnitTests
dotnet tool restore
dotnet stryker
```

Settings are in `stryker-config.json` next to the project: it mutates `DoseLab.Application` and runs the unit tests. A run takes about a minute. At the end it prints the path of the HTML report: open it.

| Status | Meaning |
| --- | --- |
| Killed | A test failed: good |
| Survived | No test failed: a missing or weak test |
| No coverage | No test even ran this code |
| Timeout | The mutant made the code loop; counted as killed |
| Ignored / Compile error | Not counted |

Useful options:

```powershell
dotnet stryker --mutate "**/TaperPlanService.cs"     # only one file
dotnet stryker --since:main                          # only code changed since main (fast, for pull requests)
```

Docs: https://stryker-mutator.io/docs/stryker-net/introduction/
