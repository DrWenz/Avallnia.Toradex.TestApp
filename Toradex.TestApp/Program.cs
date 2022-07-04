using System;
using System.Linq;
using Avalonia;
using Avalonia.LinuxFramebuffer.Output;
using Avalonia.OpenGL;
using Avalonia.ReactiveUI;

namespace Toradex.TestApp;

internal class Program
{
    internal static DateTime StartTime;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        try
        {
            PerformanceCounter.Step("Application start ...");
            var app = BuildAvaloniaApp();
            if (args.Contains("--drm"))
            {
                var drmOutput = new DrmOutput(GetArgumentValue(args, "--card", "/dev/dri/card0")) { Scaling = 1 };
                using (var ctx = drmOutput.CreateGlRenderTarget().BeginDraw())
                {
                    ctx.Context.GlInterface.ClearColor(1, 0, 1, 1);
                    ctx.Context.GlInterface.Clear(GlConsts.GL_COLOR_BUFFER_BIT | GlConsts.GL_STENCIL_BUFFER_BIT);
                }

                PerformanceCounter.Step("GL cleared.");

                return app.StartLinuxDirect(args, drmOutput);
            }

            if (args.Contains("--fbdev")) app.StartLinuxFbDev(args);

            return app
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            if (e.InnerException != null)
                Console.WriteLine(e.InnerException);
            return -1;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseFluentTheme()
            .UseReactiveUI();
    }

    private static string GetArgumentValue(string[] args, string parameter, string defaultValue = "")
    {
        foreach (var arg in args)
            if (arg.StartsWith(parameter))
                return arg.Remove(0, parameter.Length + 1);

        return defaultValue;
    }

    private static double GetArgumentValue(string[] args, string parameter, double defaultValue = 1)
    {
        foreach (var arg in args)
            if (arg.StartsWith(parameter))
            {
                if (double.TryParse(arg.Remove(0, parameter.Length + 1), out var value))
                    return value;
                return defaultValue;
            }

        return defaultValue;
    }
}