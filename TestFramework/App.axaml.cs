using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TestFramework.Application;
using TestFramework.Infrastructure;
using TestFramework.Presentation.Presenters;
using TestFramework.Presentation.Views;

namespace TestFramework.Presentation;

public class App : Avalonia.Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var configLoader = new JsonConfigLoader();
            var testRunner   = new CalculatorTestRunner();
            var executionSvc = new TestExecutionService(configLoader, testRunner);

            var window = new MainWindow();
            var _      = new MainPresenter(window, executionSvc);

            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}