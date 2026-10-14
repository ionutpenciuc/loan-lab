using System.Globalization;
using DoseLab.Application;

namespace DoseLab.Desktop;

/// <summary>
/// A small Windows desktop screen: preview a taper schedule. It uses the same rules as the API.
/// Every control has a Name. Windows UI Automation shows it as the AutomationId, which UI tests use to find it.
/// </summary>
public sealed class TaperPreviewForm : Form
{
    private readonly TaperPlanService _service = new(
        new InMemoryTaperPlanRepository(SimulatedLatency.None),
        new InMemoryMedicationCatalog(SimulatedLatency.None),
        TimeProvider.System);

    private readonly ComboBox _medication = new() { Name = "medicationComboBox", DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
    private readonly TextBox _dose = new() { Name = "doseTextBox", Width = 200 };
    private readonly TextBox _weeks = new() { Name = "weeksTextBox", Width = 200 };
    private readonly Button _preview = new() { Name = "previewButton", Text = "Preview schedule", AutoSize = true };
    private readonly Label _error = new() { Name = "errorLabel", ForeColor = Color.DarkRed, AutoSize = true };
    private readonly ListBox _schedule = new() { Name = "scheduleListBox", Width = 420, Height = 220 };
    private readonly Label _total = new() { Name = "totalLabel", AutoSize = true };

    public TaperPreviewForm()
    {
        Name = "taperPreviewWindow";
        Text = "Dose lab — taper preview";
        Width = 480;
        Height = 520;

        var medications = _service.Medications();
        _medication.DisplayMember = nameof(Medication.Name);
        _medication.DataSource = medications.ToList();

        var layout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(12), WrapContents = false };
        layout.Controls.AddRange([
            new Label { Text = "Medication", AutoSize = true }, _medication,
            new Label { Text = "Starting daily dose (mg)", AutoSize = true }, _dose,
            new Label { Text = "Number of weeks", AutoSize = true }, _weeks,
            _preview, _error, _schedule, _total,
        ]);
        Controls.Add(layout);

        _preview.Click += (_, _) => ShowPreview();
        AcceptButton = _preview;
    }

    private void ShowPreview()
    {
        _error.Text = "";
        _schedule.Items.Clear();
        _total.Text = "";

        // The lab always uses "." for decimals, whatever the Windows region settings say.
        if (!decimal.TryParse(_dose.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var dose))
        {
            _error.Text = "Starting daily dose must be a number, for example 12.5.";
            return;
        }

        if (!int.TryParse(_weeks.Text, NumberStyles.None, CultureInfo.InvariantCulture, out var weeks))
        {
            _error.Text = "Number of weeks must be a whole number.";
            return;
        }

        try
        {
            var medication = (Medication)_medication.SelectedItem!;
            var schedule = _service.Preview(medication.Code, dose, weeks);

            foreach (var week in schedule.Weeks)
                _schedule.Items.Add(string.Create(CultureInfo.InvariantCulture,
                    $"Week {week.Week}: {week.DailyDoseMg:0.00} mg daily, {week.WeeklyTotalMg:0.00} mg weekly"));

            _total.Text = string.Create(CultureInfo.InvariantCulture, $"Total: {schedule.TotalMg:0.00} mg");
        }
        catch (TaperValidationException exception)
        {
            _error.Text = exception.Message;
        }
    }
}
