using TestFramework.Application;
using TestFramework.Core.Interfaces;
using TestFramework.Core.Models;

namespace TestFramework.Presentation.Presenters;

/// <summary>
/// MVP Presenter: handles all business logic triggered by the View.
/// The View knows nothing about TestSuites or CalculatorTestRunner.
/// </summary>
public class MainPresenter
{
    private readonly IMainView _view;
    private readonly TestExecutionService _executionService;
    private IReadOnlyList<TestSuite> _loadedSuites = [];

    public MainPresenter(IMainView view, TestExecutionService executionService)
    {
        _view = view;
        _executionService = executionService;

        _view.LoadConfigsRequested += OnLoadConfigsRequested;
        _view.RunTestsRequested += OnRunTestsRequested;
    }

    private void OnLoadConfigsRequested(object? sender, EventArgs e)
    {
        var filePaths = _view.GetSelectedFilePaths();
        if (filePaths.Length == 0)
        {
            _view.SetStatus("No files selected.");
            return;
        }

        try
        {
            _loadedSuites = _executionService.LoadSuites(filePaths);
            _view.SetLoadedFiles(filePaths.Select(Path.GetFileName).ToList()!);
            _view.SetStatus($"Loaded {_loadedSuites.Count} suite(s) with {_loadedSuites.Sum(s => s.Tests.Count)} test(s). Ready to run.");
        }
        catch (Exception ex)
        {
            _view.SetStatus($"Error loading configs: {ex.Message}");
        }
    }

    private void OnRunTestsRequested(object? sender, EventArgs e)
    {
        if (_loadedSuites.Count == 0)
        {
            _view.SetStatus("No suites loaded. Please load config files first.");
            return;
        }

        try
        {
            var results = _executionService.RunSuites(_loadedSuites);
            _view.DisplayResults(results);

            int passed = results.Count(r => r.Status == Core.Enums.TestStatus.Pass);
            int failed = results.Count(r => r.Status == Core.Enums.TestStatus.Fail);
            int errors = results.Count(r => r.Status == Core.Enums.TestStatus.Error);

            _view.SetStatus($"Done — {passed} passed, {failed} failed, {errors} errors out of {results.Count} tests.");
        }
        catch (Exception ex)
        {
            _view.SetStatus($"Error running tests: {ex.Message}");
        }
    }
}