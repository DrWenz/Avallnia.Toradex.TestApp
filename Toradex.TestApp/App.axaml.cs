using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Toradex.TestApp.Views;

namespace Toradex.TestApp;

public class App : Application
{
    public override void Initialize()
    {
        PerformanceCounter.Step("App-Initialize");
        AvaloniaXamlLoader.Load(this);
        PerformanceCounter.Step("App AvaloniaXamlLoader loaded");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        AvaloniaLocator.CurrentMutable
            .Bind<IFontManagerImpl>().ToConstant(new CustomFontManagerImpl());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            singleView.MainView = new Viewbox
            {
                Child = new MainView()
            };
        base.OnFrameworkInitializationCompleted();
        StartServices();
    }

    private void StartServices()
    {
        // TODO
    }
}