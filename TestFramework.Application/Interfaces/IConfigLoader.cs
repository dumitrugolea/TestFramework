using TestFramework.Core.Models;

namespace TestFramework.Core.Interfaces;

/// <summary>
/// Contract for loading test suites from a configuration source.
/// Implement this interface to support new formats (XML, YAML, etc.).
/// </summary>
public interface IConfigLoader
{
    TestSuite Load(string filePath);
    IReadOnlyList<TestSuite> LoadAll(IEnumerable<string> filePaths);
}