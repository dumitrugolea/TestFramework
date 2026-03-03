# Calculator Test Framework

A cross-platform test framework for the `Calculator` class, built with .NET 10, Avalonia UI, and Clean Architecture (MVP pattern).

## Setup Instructions

### Requirements
- .NET 10 SDK
- Windows, Linux, or macOS

### Run the application
```bash
dotnet run --project TestFramework/TestFramework.csproj
```

> On Linux, if the window doesn't appear, prefix with `DISPLAY=:1`:
> ```bash
> DISPLAY=:1 dotnet run --project TestFramework/TestFramework.csproj
> ```

### How to use
1. Click **"Browse & Load JSON files..."** and select one or more `.json` config files from `TestConfigs/`.
2. The loaded file names appear in the list.
3. Click **"Run Tests"** to execute all test cases.
4. Results appear in the grid, color-coded:
   - **Green** — Pass
   - **Red**   — Fail
   - **Yellow** — Error (unexpected exception)

---

## Architecture

The solution follows **Clean Architecture** with **physically separate projects** for each layer — dependency rules are enforced at compile time, not just by convention.

```
TestFramework.sln
├── TestFramework.Domain/          Class library — zero external dependencies
│   ├── Calculator/                Refactored Calculator class (bug fixes applied)
│   ├── Enums/                     TestStatus (Pass, Fail, Error)
│   └── Models/                    TestCase, TestSuite, TestResult (C# records)
│
├── TestFramework.Application/     Class library — references Domain only
│   ├── Interfaces/                IConfigLoader, ITestRunner
│   └── Services/                  TestExecutionService
│
├── TestFramework.Infrastructure/  Class library — references Application + Domain
│   ├── JsonConfigLoader           Reads JSON using System.Text.Json
│   └── CalculatorTestRunner       Executes tests against the Calculator
│
└── TestFramework/  (Presentation) Avalonia WinExe — composition root
    ├── Presentation/Interfaces/   IMainView
    ├── Presentation/Presenters/   MainPresenter — all business logic for the UI
    ├── Presentation/Views/        MainWindow — only rendering and events (AXAML)
    ├── App.axaml / App.axaml.cs   Avalonia application entry point
    ├── Program.cs                 Dependency wiring + app startup
    └── TestConfigs/               basic_tests.json, edge_cases.json
```

### Dependency graph

```
Domain  ←──  Application  ←──  Infrastructure
  ↑               ↑                  ↑
  └───────────────┴──────────────────┘
              Presentation (composition root)
```

Each inner layer has **no knowledge** of outer layers. If `Domain` tried to import `Infrastructure`, the solution would not compile.

### Layer responsibilities

| Layer | Role |
|---|---|
| **Domain** | Pure domain — Calculator, models, enums. No external dependencies. |
| **Application** | Interfaces + orchestration service. Knows nothing about JSON or UI. |
| **Infrastructure** | Concrete I/O: reads JSON, runs tests against Calculator. |
| **Presentation** | MVP: `MainWindow` (View) raises events; `MainPresenter` handles them. |

### Extensibility
- **New config format** (XML, YAML): implement `IConfigLoader`, wire it in `App.axaml.cs`.
- **New class to test**: implement `ITestRunner`, wire it in `App.axaml.cs`.
- **New UI**: implement `IMainView` on any window — the Presenter works unchanged.

---

## Bug Fixes Applied to Calculator

| # | Bug | Fix |
|---|---|---|
| 1 | `Power(2.5, 3.5)` logged as `Power(2, 3)` — double cast to int | `AddToHistory` now accepts `double[]` |
| 2 | `SquareRoot(4.9)` logged as `SquareRoot(4)` — same issue | Same fix |
| 3 | `GetHistory()` exposed internal list (external code could modify it) | Returns `IReadOnlyList<string>` via `.AsReadOnly()` |

---

## JSON Configuration Format

```json
{
  "suiteName": "My Test Suite",
  "tests": [
    {
      "name": "Add two numbers",
      "operation": "Add",
      "parameters": [3, 5],
      "expectedResult": 8
    },
    {
      "name": "Divide by zero",
      "operation": "Divide",
      "parameters": [10, 0],
      "expectedResult": 0,
      "expectException": true,
      "expectedExceptionMessage": "Cannot divide by zero"
    }
  ]
}
```

### Supported operations
`Add`, `Subtract`, `Multiply`, `Divide`, `Power`, `SquareRoot`