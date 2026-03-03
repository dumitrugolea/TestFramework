namespace TestFramework.Core.Models;

/// <summary>
/// Represents a single test case loaded from a JSON configuration file.
/// </summary>
public record TestCase
{
    public string Name { get; init; } = string.Empty;
    public string Operation { get; init; } = string.Empty;
    public double[] Parameters { get; init; } = [];
    public double ExpectedResult { get; init; }
    public bool ExpectException { get; init; } = false;
    public string? ExpectedExceptionMessage { get; init; }
}