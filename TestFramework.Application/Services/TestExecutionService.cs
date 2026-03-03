using TestFramework.Core.Interfaces;
using TestFramework.Core.Models;

namespace TestFramework.Application;

/// <summary>
/// Orchestrates loading configs and running tests.
/// The Presenter calls this service — it does not know about UI or JSON details.
/// </summary>
public class TestExecutionService
{
    private readonly IConfigLoader _configLoader;
    private readonly ITestRunner _testRunner;

    public TestExecutionService(IConfigLoader configLoader, ITestRunner testRunner)
    {
        _configLoader = configLoader;
        _testRunner = testRunner;
    }

    public IReadOnlyList<TestSuite> LoadSuites(IEnumerable<string> filePaths)
        => _configLoader.LoadAll(filePaths);

    public IReadOnlyList<TestResult> RunSuites(IEnumerable<TestSuite> suites)
        => _testRunner.RunAll(suites);
}