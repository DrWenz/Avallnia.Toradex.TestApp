using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Toradex.TestApp.Views;

public partial class MainView : UserControl
{
    private bool _isFirstTimeRendered = true;

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
        base.OnAttachedToVisualTree(e);
    }

    public override void Render(DrawingContext context)
    {
        if (_isFirstTimeRendered)
        {
            _isFirstTimeRendered = false;
            Program.StaticLottieSplashToDrm?.Dispose();
            PerformanceCounter.Step("First time rendered");
        }

        base.Render(context);
    }
}