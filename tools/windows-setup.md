# Windows setup: the basics

Do this on day 1. About 45 minutes, mostly downloads.

## 1. Install

Open **PowerShell** (Start → type "PowerShell"). Run one line at a time:

```powershell
winget install --id Git.Git -e                      # Git, and Git Bash
winget install --id Microsoft.DotNet.SDK.10 -e      # .NET 10 SDK
winget install --id OpenJS.NodeJS.LTS -e            # Node.js (LTS)
winget install k6 --source winget                   # k6 load testing (lesson 05)
```

Close PowerShell, open it again, and check:

```powershell
git --version
dotnet --version        # must start with 10.
node --version          # 20 or newer
k6 version
```

Links if winget fails: https://git-scm.com/download/win · https://dotnet.microsoft.com/download/dotnet/10.0 · https://nodejs.org · https://grafana.com/docs/k6/latest/set-up/install-k6/

## 2. An IDE (pick one)

| IDE | Cost | Notes |
| --- | --- | --- |
| **Visual Studio Community** | Free for individuals and learning | The standard on Windows. In the installer choose the workload **ASP.NET and web development** and **.NET desktop development**. https://visualstudio.microsoft.com/vs/community/ |
| **JetBrains Rider** | Free for non-commercial use | Same as the mentor uses. `winget install --id JetBrains.Rider -e` |
| **VS Code** | Free | Lighter. Add the **C# Dev Kit** extension. `winget install --id Microsoft.VisualStudioCode -e` |

Open `backend/DoseLab.slnx` in Visual Studio or Rider. Tests appear in **Test Explorer** (Visual Studio: Test → Test Explorer; Rider: Unit Tests window).

## 3. Git settings (once)

```powershell
git config --global user.name "Your Name"
git config --global user.email "you@example.com"
git config --global core.autocrlf true      # Windows line endings in your editor, Unix in the repo
git config --global core.longpaths true     # node_modules has long paths
```

## 4. Which terminal?

- **PowerShell** works for every `dotnet`, `npm` and `k6` command.
- **Git Bash** (installed with Git) is needed for the `.sh` scripts, for example `instructor/bug.sh`.
- Environment variables differ:
  - PowerShell: `$env:Lab__SimulatedLatencyMs = "50"` (stays until you close the window)
  - Git Bash: `Lab__SimulatedLatencyMs=50 dotnet run ...` (only for that command)

## 5. First run

Follow the main [README](../README.md) "Start the app". If `dotnet test` reports Docker errors, either install Docker ([docker-and-postgres.md](docker-and-postgres.md)) or run `dotnet test --filter "Category!=Docker"` for now.

## Common problems

| Problem | Fix |
| --- | --- |
| `dotnet` not found after install | Close all terminals, open a new one. Restart Windows if needed. |
| `running scripts is disabled on this system` (npm in PowerShell) | `Set-ExecutionPolicy -Scope CurrentUser RemoteSigned` |
| Windows Firewall asks about `dotnet` or `node` | Allow on **private** networks |
| Port 5080 in use | `netstat -ano | findstr :5080`, then end that process id in Task Manager |
| Antivirus makes builds very slow | Ask IT to exclude the repo folder and `%USERPROFILE%\.nuget` |
