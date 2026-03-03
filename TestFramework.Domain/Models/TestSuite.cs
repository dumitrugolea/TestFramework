namespace TestFramework.Core.Models;

/// <summary>
/// Represents a collection of test cases loaded from a single JSON file.
/// </summary>
public record TestSuite
{
    public string SuiteName { get; init; } = string.Empty;
    public string SourceFile { get; init; } = string.Empty;
    public IReadOnlyList<TestCase> Tests { get; init; } = [];
}