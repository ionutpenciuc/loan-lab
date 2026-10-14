# 15 — Desktop UI automation (FlaUI, Ranorex, C++ apps) and Cypress

Time: 2 hours. The hands-on part needs Windows.

Goal: automate a Windows desktop app with C# (FlaUI), know how Ranorex does the same, and know how this applies to C++ desktop apps. Then compare Cypress with Playwright.

The job ad names "C++ desktop, .NET and modern web estates" and "Ranorex, Playwright, Cypress". Expect at least one desktop question.

## Learn: how desktop UI automation works

Windows has an accessibility layer called **UI Automation (UIA)**. Screen readers use it, and so do test tools. Every window, button and text box is an **automation element** with properties:

| Property | Example | Good for finding elements? |
| --- | --- | --- |
| AutomationId | `doseTextBox` | **Best.** Set by developers. Stable across languages and layouts. |
| Name | `Preview schedule` | OK, but changes with translations |
| ControlType | `Button`, `Edit`, `ComboBox`, `List` | Together with another property |
| ClassName | `WindowsForms10.EDIT...` | Avoid: a framework detail |
| Position or index | "the 3rd text box" | Never: breaks on any layout change |

Elements offer **patterns**: Invoke (click), Value (set text), SelectionItem (choose), ExpandCollapse, Grid.

UIA works for WinForms, WPF, WinUI and **C++ apps** (Win32, MFC, and Qt with its accessibility support). The most useful request a tester can make to C++ developers: **give every control you want tested a stable automation id or accessible name.** It is the desktop version of `data-testid`.

| Tool | Language | Cost | Notes |
| --- | --- | --- | --- |
| **FlaUI** | C# | Free | Wraps UIA. Used in this lesson. |
| **Ranorex Studio** | C# / VB.NET, plus a recorder | Commercial, free trial | Named in the ad. See [tools/ranorex.md](../tools/ranorex.md). |
| WinAppDriver / Appium Windows driver | Any language (WebDriver) | Free | Found in older suites |
| Squish, TestComplete | Several | Commercial | Squish is common for Qt / C++ apps |

For C++ **unit** tests: GoogleTest or Catch2, run with CTest in the pipeline. The pyramid is the same as in .NET.

## Learn: the same rules as on the web

| Web (Playwright) | Desktop (FlaUI / Ranorex) |
| --- | --- |
| Role and label locators, `data-testid` | AutomationId |
| Page object | Screen object (`Screens/TaperPreviewScreen.cs`) |
| Web-first assertions that wait | `Retry.WhileEmpty(...)` in FlaUI; Ranorex waits for elements by default |
| No `waitForTimeout` | No `Thread.Sleep` |
| New browser context per test | New app process per test (`DesktopApp`) |
| Trace and screenshot on failure | Screenshot on failure, Ranorex report |

Desktop-specific points:

- Tests drive the **real** mouse and keyboard. Do not use the PC while they run.
- A CI agent for desktop UI tests needs an **interactive, unlocked desktop session**. Hosted Windows runners (GitHub `windows-latest`) provide one.
- **Regional settings.** Windows in Romanian uses `,` as the decimal separator. Number input and output on desktop apps often break here. Always test with the users' real regional settings.
- Screen resolution and display scaling (125%, 150%) can move or hide controls.

## Learn: Ranorex in five points

1. **Ranorex Spy** shows the element tree and builds a **RanoreXPath** for an element, for example `/form[@automationid='taperPreviewWindow']/text[@automationid='doseTextBox']`.
2. The **repository** stores elements with readable names. Tests use the names, not the paths. It is a built-in page object.
3. The **recorder** creates test modules from clicks. Like Playwright codegen: a start, not the final code.
4. Code modules are **C#**. Everything you learned about C# and screen objects applies.
5. Test suites, data-driven runs from Excel or CSV, and an HTML report. It runs from the command line, so CI (GitLab, Jenkins, Azure DevOps) can start it.

## In this repo

| Path | What |
| --- | --- |
| `desktop/src/DoseLab.Desktop/TaperPreviewForm.cs` | A WinForms screen. It uses the same rules as the API. Look at the `Name` of each control. |
| `desktop/tests/DoseLab.Desktop.UiTests/Screens/` | `DesktopApp` (starts and closes the app), `TaperPreviewScreen` (screen object) |
| `desktop/tests/DoseLab.Desktop.UiTests/TaperPreviewScreenShould.cs` | A sample test and exercises L15-1 to L15-4 |
| `.github/workflows/ci.yml`, job `desktop-ui` | Runs the tests on a Windows runner |

## Do

Install: [tools/flaui-and-desktop.md](../tools/flaui-and-desktop.md).

1. **Run the app.** From `desktop`: `dotnet run --project src/DoseLab.Desktop`. Preview Calmafen, 10 mg, 3 weeks.
2. **Inspect it.** Open **Accessibility Insights for Windows** (or FlaUInspect). Hover over the dose box. Find its AutomationId, ControlType and patterns. Find a control with **no** useful AutomationId (hint: the labels).
3. **Run the tests.** From `desktop`: `dotnet test DoseLab.Desktop.slnx`. Watch the window. Then read `TaperPreviewScreen.cs`: how does it wait?
4. **L15-1** to **L15-3.** Remove `Skip`, see the test fail, write it, make it pass. For L15-1 answer the question about checking for "no rows".
5. **L15-4: error guessing.** Write all the cases. Then change your Windows region: Settings → Time & language → Language & region → Regional format → **Romanian (Romania)**. Restart the app and type `1,5` by hand. Is the result what a Romanian nurse expects? If you find a bug, write the report in `notes/bugs.md` with the lesson 10 template, including the patient risk and the severity. Change the region back after.
6. **Screenshot on failure.** In `DesktopApp.Dispose()` (or a test helper), save a screenshot when a test fails. Hint: `FlaUI.Core.Capturing.Capture.Screen().ToFile(path)`.
7. **Ranorex (optional, 30 min).** Install the trial (see [tools/ranorex.md](../tools/ranorex.md)). Use Ranorex Spy on the desktop app, record the L15-1 scenario, and compare the RanoreXPath with your FlaUI code.

## Learn: Cypress vs Playwright

| | Playwright | Cypress |
| --- | --- | --- |
| How it runs | Drives browsers from outside (Node.js process) | Runs inside the browser, next to the app |
| Languages | TypeScript/JS, **C#**, Java, Python | JavaScript / TypeScript only |
| Browsers | Chromium, Firefox, WebKit (Safari engine) | Chromium family, Firefox; WebKit experimental |
| Several tabs, windows, domains | Yes | Limited (`cy.origin` for other domains) |
| Waiting | Auto-wait + web-first `expect` | Auto-retry of commands and assertions |
| Network mocking | `page.route` | `cy.intercept` |
| Parallel runs | Built in (workers, shards) | Through Cypress Cloud or by hand |
| Debugging | UI mode, trace viewer | Time-travel runner (very good) |
| Component tests | Experimental | Mature |

The same test in both:

```ts
// Playwright
await page.getByRole('button', { name: 'Add taper plan' }).click()
await page.getByLabel('Patient name').fill('Ana Pop')
await expect(page.getByRole('cell', { name: 'Ana Pop' })).toBeVisible()

// Cypress (with @testing-library/cypress for findByRole / findByLabelText)
cy.findByRole('button', { name: 'Add taper plan' }).click()
cy.findByLabelText('Patient name').type('Ana Pop')
cy.findByRole('cell', { name: 'Ana Pop' }).should('be.visible')
```

Cypress commands are queued, not awaited. You cannot write `const text = cy.get(...)`; use `.then(...)` or aliases.

Optional: [tools/cypress.md](../tools/cypress.md) shows how to try Cypress on this app in 15 minutes.

## Check yourself

1. What is UI Automation, and why does it work for both .NET and C++ apps?
2. Which element property do you use first, and why?
3. What do you ask C++ developers for, to make their app testable?
4. Why do desktop UI tests need an interactive session in CI?
5. Name two desktop-only risks that web tests do not have.
6. What does the Ranorex repository give you, compared with a FlaUI screen object?
7. Playwright vs Cypress: three differences.

## Say it in the interview

- "Desktop automation follows the same rules as web: stable ids (AutomationIds), screen objects, no sleeps, a fresh app per test. I've done it with FlaUI in C#; Ranorex uses the same UI Automation layer, with a repository and recorder on top."
- "For C++ apps I'd push for automation ids on controls early. It's the cheapest testability win."
- "I test desktop apps with the users' regional settings. In my lab, that found a dose-entry bug: <say what you found in L15-4, and its patient risk>."
- "I prefer Playwright for new web suites: more browsers, C# support, parallel runs built in. I can read and maintain Cypress suites."
