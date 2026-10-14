// Answer key for lesson 15. Copy into desktop/tests/DoseLab.Desktop.UiTests/ to run it (Windows only).
// It also needs two methods in Screens/TaperPreviewScreen.cs:
//
//     public string Total()
//     {
//         var text = Retry.WhileEmpty(() => ById("totalLabel").AsLabel().Text, Timeout, throwOnTimeout: false).Result;
//         return text ?? "";
//     }
//
//     public IReadOnlyList<string> ScheduleRowsNow() =>
//         ById("scheduleListBox").AsListBox().Items.Select(item => item.Text).ToArray();
//
// L15-1 question: ScheduleRows() waits 5 s and then returns empty, so "no rows" passes but is slow,
// and it could pass too early if rows appeared late. Better: wait for the error first (it proves the click
// was handled), then read the rows once without waiting.
//
// L15-4: "1,5" is a planted, always-on bug. The app parses with the invariant culture and allows thousands
// separators, so "1,5" becomes 15 mg (a ten-times dose) with no error. On a Romanian Windows a nurse types a comma.
// Expected answer: a bug report, severity High/Critical (patient harm), and the case marked Skip = "Known bug: ...".
// Fixes to discuss: reject thousands separators, accept the user's culture, or show the parsed dose for confirmation.
using DoseLab.Desktop.UiTests.Screens;
using FluentAssertions;
using Xunit;

namespace DoseLab.Desktop.UiTests;

[Trait("Category", "DesktopUi")]
public sealed class TaperPreviewScreenSolutionShould : IDisposable
{
    private readonly DesktopApp _app = new();

    public void Dispose() => _app.Dispose();

    [Fact]
    public void ShowAnErrorForADoseAboveTheMedicationMaximum()
    {
        // execute

        var screen = _app.Screen.ChooseMedication("Calmafen").EnterStartingDose("40.01").EnterWeeks("3").Preview();

        // verify

        screen.Error().Should().Be("Starting daily dose must not exceed 40 mg for Calmafen.");
        screen.ScheduleRowsNow().Should().BeEmpty();
    }

    [Fact]
    public void ClearTheOldScheduleWhenTheNextPreviewFails()
    {
        // setup

        var screen = _app.Screen.ChooseMedication("Calmafen").EnterStartingDose("10").EnterWeeks("3").Preview();
        screen.ScheduleRows().Should().HaveCount(3);

        // execute

        screen.EnterWeeks("53").Preview();

        // verify

        screen.Error().Should().Be("Number of weeks must be between 1 and 52.");
        screen.ScheduleRowsNow().Should().BeEmpty();
    }

    [Fact]
    public void ShowTheTotalForTheWholeTaper()
    {
        // execute

        var screen = _app.Screen.ChooseMedication("Calmafen").EnterStartingDose("10").EnterWeeks("3").Preview();

        // verify

        // step 3.33; daily 10, 6.67, 3.34; weekly 70, 46.69, 23.38; total 140.07
        screen.Total().Should().Be("Total: 140.07 mg");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("-5")]
    [InlineData("1e3")]
    [InlineData("1,5", Skip = "Known bug: a decimal comma is read as a thousands separator (1,5 → 15 mg)")]
    public void RejectADoseThatIsNotAValidNumber(string dose)
    {
        // execute

        var screen = _app.Screen.ChooseMedication("Steriva").EnterStartingDose(dose).EnterWeeks("4").Preview();

        // verify

        screen.Error().Should().NotBeEmpty();
        screen.ScheduleRowsNow().Should().BeEmpty();
    }
}
