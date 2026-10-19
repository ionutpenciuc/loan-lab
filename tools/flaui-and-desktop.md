# Desktop UI automation tools (Windows)

Needed for lesson 15.

## The app and the tests

Nothing extra to install: FlaUI is a NuGet package in `desktop/tests/DoseLab.Desktop.UiTests`. From `desktop`:

```powershell
dotnet run --project src/DoseLab.Desktop          # the app
dotnet test DoseLab.Desktop.slnx                  # the UI tests: do not touch mouse or keyboard while they run
```

In Visual Studio or Rider, open `desktop/DoseLab.Desktop.slnx`.

The desktop projects build on macOS and Linux (for the mentor and CI), but they only **run** on Windows.

## Inspect elements

Install one of these to see automation ids, control types and patterns:

- **Accessibility Insights for Windows** (Microsoft, free): `winget install --id Microsoft.AccessibilityInsights -e`, or https://accessibilityinsights.io/downloads/. Use **Inspect** mode and hover over an element.
- **FlaUInspect** (free): download the zip from https://github.com/FlaUI/FlaUInspect/releases, run `FlaUInspect.exe`, choose UIA3.

## Change the Windows regional format (L15-4)

Settings → Time & language → Language & region → **Regional format** → Romanian (Romania). Restart the app. Change it back after the exercise.

## Common problems

| Problem | Fix |
| --- | --- |
| `Build the desktop solution first` | Run `dotnet build DoseLab.Desktop.slnx` from `desktop` |
| Test cannot find an element | Check the AutomationId with an inspector. Was the window still loading? |
| Tests fail when the screen is locked or the laptop sleeps | UI tests need an unlocked, awake desktop |
| Display scaling above 100% moves things | Prefer AutomationIds over positions (never click by coordinates) |
