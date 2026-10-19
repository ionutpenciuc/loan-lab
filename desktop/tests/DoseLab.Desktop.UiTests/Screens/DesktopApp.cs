using FlaUI.Core;
using FlaUI.UIA3;

namespace DoseLab.Desktop.UiTests.Screens;

/// <summary>Starts DoseLab.Desktop.exe for one test, and closes it after.</summary>
public sealed class DesktopApp : IDisposable
{
    private readonly Application _application;
    private readonly UIA3Automation _automation = new();

    public DesktopApp()
    {
        _application = Application.Launch(ExePath());
        var window = _application.GetMainWindow(_automation, TimeSpan.FromSeconds(15))
            ?? throw new InvalidOperationException("The Dose lab window did not open.");
        Screen = new TaperPreviewScreen(window);
    }

    public TaperPreviewScreen Screen { get; }

    public void Dispose()
    {
        _application.Close();
        _application.Dispose();
        _automation.Dispose();
    }

    // The tests run from desktop/tests/DoseLab.Desktop.UiTests/bin/<Configuration>/<TargetFramework>/.
    // The app is built to desktop/src/DoseLab.Desktop/bin/<Configuration>/<TargetFramework>/.
    private static string ExePath()
    {
        var output = new DirectoryInfo(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar));
        var desktopRoot = output.Parent!.Parent!.Parent!.Parent!.Parent!;
        var exe = Path.Combine(desktopRoot.FullName, "src", "DoseLab.Desktop", "bin",
            output.Parent!.Name, output.Name, "DoseLab.Desktop.exe");

        return File.Exists(exe) ? exe : throw new FileNotFoundException("Build the desktop solution first.", exe);
    }
}
