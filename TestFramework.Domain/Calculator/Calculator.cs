namespace TestFramework.Calculator;

/// <summary>
/// Performs basic arithmetic operations and maintains an operation history.
/// Bug fixes applied:
///   1. AddToHistory now accepts double[] to avoid precision loss for Power/SquareRoot.
///   2. GetHistory returns a read-only copy to protect internal state.
/// </summary>
public class Calculator
{
    private readonly List<string> _history = new();

    public int Add(params int[] numbers)
    {
        int result = 0;
        foreach (var number in numbers)
            result += number;

        AddToHistory("Add", numbers.Select(n => (double)n).ToArray(), result);
        return result;
    }

    public int Subtract(int a, int b)
    {
        int result = a - b;
        AddToHistory("Subtract", [a, b], result);
        return result;
    }

    public int Multiply(params int[] numbers)
    {
        int result = 1;
        foreach (var number in numbers)
            result *= number;

        AddToHistory("Multiply", numbers.Select(n => (double)n).ToArray(), result);
        return result;
    }

    public double Divide(int a, int b)
    {
        if (b == 0)
            throw new DivideByZeroException("Cannot divide by zero.");

        double result = (double)a / b;
        AddToHistory("Divide", [a, b], result);
        return result;
    }

    /// <summary>
    /// Bug fix: operands logged as double to preserve precision (e.g. Power(2.5, 3.5)).
    /// </summary>
    public double Power(double baseNumber, double exponent)
    {
        double result = Math.Pow(baseNumber, exponent);
        AddToHistory("Power", [baseNumber, exponent], result);
        return result;
    }

    /// <summary>
    /// Bug fix: operand logged as double to preserve precision (e.g. SquareRoot(4.9)).
    /// </summary>
    public double SquareRoot(double number)
    {
        if (number < 0)
            throw new ArgumentOutOfRangeException(nameof(number), "Cannot calculate square root of a negative number.");

        double result = Math.Sqrt(number);
        AddToHistory("SquareRoot", [number], result);
        return result;
    }

    /// <summary>
    /// Bug fix: returns a copy to prevent external modification of internal history.
    /// </summary>
    public IReadOnlyList<string> GetHistory() => _history.AsReadOnly();

    private void AddToHistory(string operation, double[] operands, double result)
    {
        _history.Add($"{operation}({string.Join(", ", operands)}) = {result}");
    }
}