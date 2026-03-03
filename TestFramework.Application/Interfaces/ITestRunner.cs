using TestFramework.Core.Models;

namespace TestFramework.Core.Interfaces;

/// <summary>
/// Contract for running test suites and producing results.
/// Implement this interface to support testing other classes besides Calculator.
/// </summary>
public interface ITestRunner
{
    IReadOnlyList<TestResult> Run(TestSuite suite);
    IReadOnlyList<TestResult> RunAll(IEnumerable<TestSuite> suites);
}