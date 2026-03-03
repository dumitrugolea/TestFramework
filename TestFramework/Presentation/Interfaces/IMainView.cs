using TestFramework.Core.Models;

namespace TestFramework.Core.Interfaces;

/// <summary>
/// MVP contract: the Presenter talks to the View through this interface.
/// This decouples UI logic from business logic.
/// </summary>
public interface IMainView
{
    event EventHandler LoadConfigsRequested;
    event EventHandler RunTestsRequested;

    void DisplayResults(IReadOnlyList<TestResult> results);
    void SetStatus(string message);
    void SetLoadedFiles(IReadOnlyList<string> filePaths);
    string[] GetSelectedFilePaths();
}