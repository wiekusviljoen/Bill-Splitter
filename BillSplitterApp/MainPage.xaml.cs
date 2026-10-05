namespace BillSplitterApp;

public partial class MainPage : ContentPage
{
    private double tipPercent;

    public MainPage()
    {
        InitializeComponent();
        UpdatePeopleLabel();
        Recalculate();
    }

    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                ReceiptStatus.Text = "Camera capture is not available on this device.";
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null) return;

            var stream = await photo.OpenReadAsync();
            ReceiptImage.Source = ImageSource.FromStream(() => stream);
            ReceiptImage.IsVisible = true;
            ReceiptStatus.Text = "Receipt captured. Automatic total reading is coming next.";
        }
        catch (Exception ex)
        {
            ReceiptStatus.Text = $"Could not capture receipt: {ex.Message}";
        }
    }

    private void OnTipClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && double.TryParse(button.CommandParameter?.ToString(), out var value))
        {
            tipPercent = value;
            CustomTipEntry.Text = value.ToString("0");
            Recalculate();
        }
    }

    private void OnInputChanged(object? sender, EventArgs e)
    {
        UpdatePeopleLabel();
        if (sender == CustomTipEntry && double.TryParse(CustomTipEntry.Text, out var custom))
            tipPercent = Math.Max(0, custom);
        Recalculate();
    }

    private void UpdatePeopleLabel()
    {
        var people = Math.Max(1, (int)PeopleStepper.Value);
        PeopleLabel.Text = people == 1 ? "1 person" : $"{people} people";
    }

    private void Recalculate()
    {
        var total = double.TryParse(TotalEntry.Text, out var value) ? Math.Max(0, value) : 0;
        var people = Math.Max(1, (int)PeopleStepper.Value);
        var tip = total * Math.Max(0, tipPercent) / 100.0;
        var grand = total + tip;
        var each = grand / people;

        GrandTotalLabel.Text = $"N$ {grand:N2}";
        PerPersonLabel.Text = $"N$ {each:N2}";
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        TotalEntry.Text = string.Empty;
        PeopleStepper.Value = 2;
        tipPercent = 0;
        CustomTipEntry.Text = string.Empty;
        ReceiptImage.Source = null;
        ReceiptImage.IsVisible = false;
        ReceiptStatus.Text = "Photo capture ready";
        Recalculate();
    }
}