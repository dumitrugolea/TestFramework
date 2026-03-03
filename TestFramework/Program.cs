using Avalonia;

static class Program
{
    [STAThread]
    static void Main(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<TestFramework.Presentation.App>()
            .UsePlatformDetect()
            .LogToTrace();
}