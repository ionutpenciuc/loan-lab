namespace DoseLab.Desktop;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        System.Windows.Forms.Application.Run(new TaperPreviewForm());
    }
}
