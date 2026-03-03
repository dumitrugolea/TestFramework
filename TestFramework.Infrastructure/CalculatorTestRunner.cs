using TestFramework.Core.Enums;
using TestFramework.Core.Interfaces;
using TestFramework.Core.Models;

namespace TestFramework.Infrastructure;

/// <summary>
/// Executes test cases against the Calculator class.
/// To test a different class, implement ITestRunner and register it.
/// </summary>
public class CalculatorTestRunner : ITestRunner
{
    public IReadOnlyList<TestResult> Run(TestSuite suite)
    {
        var calculator = new Calculator.Calculator();
        var results = new List<TestResult>();

        foreach (var testCase in suite.Tests)
        {
            var result = ExecuteTestCase(calculator, testCase, suite.SuiteName);
            results.Add(result);
        }

        return results.AsReadOnly();
    }

    public IReadOnlyList<TestResult> RunAll(IEnumerable<TestSuite> suites)
        => suites.SelectMany(Run).ToList().AsReadOnly();

    private static TestResult ExecuteTestCase(Calculator.Calculator calculator, TestCase testCase, string suiteName)
    {
        try
        {
            double actual = Invoke(calculator, testCase);
            bool passed = Math.Abs(actual - testCase.ExpectedResult) < 1e-9;

            return new TestResult
            {
                SuiteName = suiteName,
                TestName = testCase.Name,
                Operation = testCase.Operation,
                Parameters = string.Join(", ", testCase.Parameters),
                ExpectedResult = testCase.ExpectedResult,
                ActualResult = actual,
                Status = passed ? TestStatus.Pass : TestStatus.Fail
            };
        }
        catch (Exception ex) when (testCase.ExpectException)
        {
            bool messageMatches = testCase.ExpectedExceptionMessage is null
                || ex.Message.Contains(testCase.ExpectedExceptionMessage, StringComparison.OrdinalIgnoreCase);

            return new TestResult
            {
                SuiteName = suiteName,
                TestName = testCase.Name,
                Operation = testCase.Operation,
                Parameters = string.Join(", ", testCase.Parameters),
                ExpectedResult = testCase.ExpectedResult,
                ActualResult = null,
                Status = messageMatches ? TestStatus.Pass : TestStatus.Fail,
                ErrorMessage = messageMatches ? $"Expected exception: {ex.Message}" : $"Wrong exception message: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new TestResult
            {
                SuiteName = suiteName,
                TestName = testCase.Name,
                Operation = testCase.Operation,
                Parameters = string.Join(", ", testCase.Parameters),
                ExpectedResult = testCase.ExpectedResult,
                ActualResult = null,
                Status = TestStatus.Error,
                ErrorMessage = ex.Message
            };
        }
    }

    private static double Invoke(Calculator.Calculator calc, TestCase tc)
    {
        var p = tc.Parameters;
        return tc.Operation.ToUpperInvariant() switch
        {
            "ADD"        => calc.Add(p.Select(x => (int)x).ToArray()),
            "SUBTRACT"   => calc.Subtract((int)p[0], (int)p[1]),
            "MULTIPLY"   => calc.Multiply(p.Select(x => (int)x).ToArray()),
            "DIVIDE"     => calc.Divide((int)p[0], (int)p[1]),
            "POWER"      => calc.Power(p[0], p[1]),
            "SQUAREROOT" => calc.SquareRoot(p[0]),
            _ => throw new NotSupportedException($"Operation '{tc.Operation}' is not supported.")
        };
    }
}