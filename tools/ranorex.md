# Ranorex Studio (optional)

Ranorex is named in the job ad. It is commercial, Windows only, with a **free trial** (about 2 weeks). Start the trial close to the interview, not on day 1.

## Install

1. Request a trial at https://www.ranorex.com/free-trial/ and download Ranorex Studio.
2. Run the installer. It may ask for .NET Framework 4.8 (included in current Windows).
3. Start Ranorex Studio → New solution → **Desktop** → C#.

## A first test on the lab app

1. Build and start the lab desktop app (`dotnet run --project src/DoseLab.Desktop` from `desktop`).
2. Open **Ranorex Spy**. Track the dose box. Read its **RanoreXPath**: it uses the AutomationId (`doseTextBox`). Add it to the **repository** with a readable name.
3. **Record** the L15-1 scenario: choose Calmafen, type 40.01, 3 weeks, Preview.
4. Add a **validation** on the error label text.
5. Open the generated recording module. Then convert one step to a **code module** in C#. Compare it with your FlaUI screen object.
6. Run it. Open the **report**.

## What to say

- The repository is Ranorex's page object: one place for element paths.
- RanoreXPath with AutomationIds is stable; paths with indexes break.
- Recorded tests are a start. Maintainable suites use code modules, data binding and shared modules.
- It runs from the command line (`<TestSuite>.exe`), so GitLab CI can run it on a Windows runner with an interactive session.

Free learning: Ranorex has a free online course and user guide (https://www.ranorex.com/help/latest/).
