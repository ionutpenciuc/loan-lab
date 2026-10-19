using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;

namespace DoseLab.Desktop.UiTests.Screens;

/// <summary>
/// Screen object (the desktop version of a page object). Tests say what the user does;
/// only this class knows the AutomationIds and how to wait.
/// </summary>
public sealed class TaperPreviewScreen(Window window)
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    public TaperPreviewScreen ChooseMedication(string name)
    {
        ById("medicationComboBox").AsComboBox().Select(name);
        return this;
    }

    public TaperPreviewScreen EnterStartingDose(string dose)
    {
        ById("doseTextBox").AsTextBox().Text = dose;
        return this;
    }

    public TaperPreviewScreen EnterWeeks(string weeks)
    {
        ById("weeksTextBox").AsTextBox().Text = weeks;
        return this;
    }

    public TaperPreviewScreen Preview()
    {
        ById("previewButton").AsButton().Invoke();
        return this;
    }

    /// <summary>Waits until the schedule has rows (no fixed sleep), then returns their text.</summary>
    public IReadOnlyList<string> ScheduleRows()
    {
        var rows = Retry.WhileEmpty(
            () => ById("scheduleListBox").AsListBox().Items.Select(item => item.Text).ToArray(),
            Timeout, throwOnTimeout: false).Result;
        return rows ?? [];
    }

    /// <summary>Waits until an error is shown, then returns it. Returns "" if none appears.</summary>
    public string Error()
    {
        var text = Retry.WhileEmpty(() => ById("errorLabel").AsLabel().Text, Timeout, throwOnTimeout: false).Result;
        return text ?? "";
    }

    // TODO(L15-3): add a Total() method that reads "totalLabel", like Error() reads "errorLabel".

    private AutomationElement ById(string automationId) =>
        window.FindFirstDescendant(condition => condition.ByAutomationId(automationId))
        ?? throw new InvalidOperationException($"No element with AutomationId '{automationId}'.");
}
