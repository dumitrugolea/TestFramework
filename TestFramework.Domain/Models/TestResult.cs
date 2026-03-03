using TestFramework.Core.Enums;

namespace TestFramework.Core.Models;

/// <summary>
/// Represents the outcome of running a single test case.
/// </summary>
public record TestResult
{
    public string SuiteName { get; init; } = string.Empty;
    public string TestName { get; init; } = string.Empty;
    public string Operation { get; init; } = string.Empty;
    public string Parameters { get; init; } = string.Empty;
    public double ExpectedResult { get; init; }
    public double? ActualResult { get; init; }
    public TestStatus Status { get; init; }
    public string? ErrorMessage { get; init; }
}