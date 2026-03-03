using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using TestFramework.Core.Interfaces;
using TestFramework.Core.Models;

namespace TestFramework.Presentation.Views;

public partial class MainWindow : Window, IMainView
{
    public event EventHandler? LoadConfigsRequested;
    public event EventHandler? RunTestsRequested;

    private readonly List<string> _selectedFilePaths = [];

    public MainWindow() => InitializeComponent();

    // --- IMainView ---

    public void DisplayResults(IReadOnlyList<TestResult> results)
    {
        dgvResults.ItemsSource = results.Select(r => new ResultRow
        {
            Suite     = r.SuiteName,
            Test      = r.TestName,
            Operation = r.Operation,
            Params    = r.Parameters,
            Expected  = r.ExpectedResult,
            Actual    = r.ActualResult.HasValue ? r.ActualResult.Value.ToString("G") : "—",
            Status    = r.Status.ToString(),
            Message   = r.ErrorMessage ?? string.Empty
        }).ToList();
    }

    public void SetStatus(string message) => lblStatus.Text = message;

    public void SetLoadedFiles(IReadOnlyList<string> filePaths)
        => lstFiles.ItemsSource = filePaths.ToList();

    public string[] GetSelectedFilePaths() => _selectedFilePaths.ToArray();

    // --- UI event handlers ---

    private async void btnBrowse_Click(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title         = "Select Test Configuration Files",
            AllowMultiple = true,
            FileTypeFilter = [new FilePickerFileType("JSON Files") { Patterns = ["*.json"] }]
        });

        if (files.Count > 0)
        {
            _selectedFilePaths.Clear();
            _selectedFilePaths.AddRange(files.Select(f => f.Path.LocalPath));
            LoadConfigsRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    private void btnRun_Click(object? sender, RoutedEventArgs e)
        => RunTestsRequested?.Invoke(this, EventArgs.Empty);

    // --- Row coloring ---

    private void dgvResults_LoadingRow(object? sender, DataGridRowEventArgs e)
    {
        if (e.Row.DataContext is ResultRow row)
        {
            (e.Row.Background, e.Row.Foreground) = row.Status switch
            {
                "Pass"  => ((IBrush)new SolidColorBrush(Color.Parse("#C6EFCE")),
                            (IBrush)new SolidColorBrush(Color.Parse("#375623"))),
                "Fail"  => ((IBrush)new SolidColorBrush(Color.Parse("#FFC7CE")),
                            (IBrush)new SolidColorBrush(Color.Parse("#9C0006"))),
                "Error" => ((IBrush)new SolidColorBrush(Color.Parse("#FFEB9C")),
                            (IBrush)new SolidColorBrush(Color.Parse("#7D6608"))),
                _       => ((IBrush)Brushes.Transparent,
                            (IBrush)Brushes.Black)
            };
        }
    }

    // --- DTO for grid binding ---

    private sealed record ResultRow
    {
        public string Suite     { get; init; } = string.Empty;
        public string Test      { get; init; } = string.Empty;
        public string Operation { get; init; } = string.Empty;
        public string Params    { get; init; } = string.Empty;
        public double Expected  { get; init; }
        public string Actual    { get; init; } = string.Empty;
        public string Status    { get; init; } = string.Empty;
        public string Message   { get; init; } = string.Empty;
    }
}