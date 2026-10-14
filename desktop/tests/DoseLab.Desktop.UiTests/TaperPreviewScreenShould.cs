using DoseLab.Desktop.UiTests.Screens;
using FluentAssertions;
using Xunit;

namespace DoseLab.Desktop.UiTests;

// Lesson 15: Windows desktop UI tests with FlaUI. Windows only. Exercises: git grep "TODO(L15"
// Run from the desktop folder: dotnet test DoseLab.Desktop.slnx
// Do not touch the mouse or keyboard while they run: they drive the real window.
[Trait("Category", "DesktopUi")]
public sealed class TaperPreviewScreenShould : IDisposable
{
    private readonly DesktopApp _app = new();

    public void Dispose() => _app.Dispose();

    [Fact]
    public void PreviewAnUnevenTaperWithTwoDecimals()
    {
        // execute

        var rows = _app.Screen
            .ChooseMedication("Calmafen")
            .EnterStartingDose("10")
            .EnterWeeks("3")
            .Preview()
            .ScheduleRows();

        // verify

        rows.Should().HaveCount(3);
        rows[1].Should().StartWith("Week 2: 6.67 mg daily");
        rows[2].Should().StartWith("Week 3: 3.34 mg daily");
    }

    [Fact(Skip = "TODO(L15-1)")]
    public void ShowAnErrorForADoseAboveTheMedicationMaximum()
    {
        // TODO(L15-1): Calmafen, 40.01 mg, 3 weeks. Expect the error from the domain rules, and no schedule rows.
        // Hint: ScheduleRows() waits up to 5 s for rows. Is that a good way to check "no rows"? What would be better?
        Assert.Fail("Not written yet.");
    }

    [Fact(Skip = "TODO(L15-2)")]
    public void ClearTheOldScheduleWhenTheNextPreviewFails()
    {
        // TODO(L15-2): preview a valid plan first. Then change the weeks to 53 and preview again.
        // The old rows must be gone, and the error must be shown.
        Assert.Fail("Not written yet.");
    }

    [Fact(Skip = "TODO(L15-3)")]
    public void ShowTheTotalForTheWholeTaper()
    {
        // TODO(L15-3): add Total() to TaperPreviewScreen. Calmafen, 10 mg, 3 weeks.
        // Work out the expected total by hand from the domain rules in CLAUDE.md first.
        Assert.Fail("Not written yet.");
    }

    [Theory(Skip = "TODO(L15-4)")]
    [InlineData("abc")]
    public void RejectADoseThatIsNotAValidNumber(string dose)
    {
        // TODO(L15-4): error guessing for the dose box. A nurse may type: empty, spaces, "-5", "1e3",
        // and, on a Romanian Windows, "1,5" for one and a half mg (Steriva, 4 weeks).
        // Each must show an error and no schedule. Run them all. If one fails, it may be a real bug:
        // write a bug report in notes/bugs.md (lesson 10 template), then mark that case with Skip = "Known bug: <title>".

        // execute

        _app.Screen.ChooseMedication("Steriva").EnterStartingDose(dose).EnterWeeks("4").Preview();

        // verify

        Assert.Fail("Not written yet.");
    }
}
