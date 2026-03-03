using System.Text.Json;
using System.Text.Json.Serialization;
using TestFramework.Core.Interfaces;
using TestFramework.Core.Models;

namespace TestFramework.Infrastructure;

/// <summary>
/// Loads test suites from JSON configuration files using System.Text.Json (built-in .NET 10).
/// To support a new format, create a new class implementing IConfigLoader.
/// </summary>
public class JsonConfigLoader : IConfigLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public TestSuite Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Config file not found: {filePath}");

        string json = File.ReadAllText(filePath);
        var dto = JsonSerializer.Deserialize<TestSuiteDto>(json, Options)
                  ?? throw new InvalidDataException($"Failed to parse: {filePath}");

        return new TestSuite
        {
            SuiteName = dto.SuiteName,
            SourceFile = Path.GetFileName(filePath),
            Tests = dto.Tests.Select(MapTestCase).ToList().AsReadOnly()
        };
    }

    public IReadOnlyList<TestSuite> LoadAll(IEnumerable<string> filePaths)
        => filePaths.Select(Load).ToList().AsReadOnly();

    private static TestCase MapTestCase(TestCaseDto dto) => new()
    {
        Name = dto.Name,
        Operation = dto.Operation,
        Parameters = dto.Parameters,
        ExpectedResult = dto.ExpectedResult,
        ExpectException = dto.ExpectException,
        ExpectedExceptionMessage = dto.ExpectedExceptionMessage
    };

    // Private DTOs for deserialization
    private sealed class TestSuiteDto
    {
        public string SuiteName { get; set; } = string.Empty;
        public List<TestCaseDto> Tests { get; set; } = [];
    }

    private sealed class TestCaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public double[] Parameters { get; set; } = [];
        public double ExpectedResult { get; set; }
        public bool ExpectException { get; set; } = false;
        public string? ExpectedExceptionMessage { get; set; }
    }
}