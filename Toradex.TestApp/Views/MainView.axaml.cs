using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Rendering;

namespace Toradex.TestApp.Views;

public partial class MainView : UserControl
{
    private bool first;

    public MainView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        PerformanceCounter.Step("OnAttachedToVisualTree");
        e.Root.Renderer.DrawFps = true;
        e.Root.Renderer.SceneInvalidated += RendererOnSceneInvalidated;
        base.OnAttachedToVisualTree(e);
    }

    private void RendererOnSceneInvalidated(object? sender, SceneInvalidatedEventArgs e)
    {
        if (first)
            return;
        PerformanceCounter.Step("First time rendered");
        first = true;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Program.StaticLottieSplashToDrm?.Dispose();
    }
}