using System;
using System.Linq;
using Avalonia;
using Avalonia.LinuxFramebuffer.Output;
using Avalonia.ReactiveUI;
using SkiaSharp;
using SkiaSharp.Skottie;

namespace Toradex.TestApp;

internal class Program
{
    public static LottieSplashToDrm StaticLottieSplashToDrm { get; set; }

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
                StaticLottieSplashToDrm = new MySplash(drmOutput);
                if (Animation.TryParse(Resources.Loading, out var animation))
                {
                    animation.Seek(0);
                    StaticLottieSplashToDrm.Load(animation);
                }

                PerformanceCounter.Step("Splashscreen created.");
                return app.StartLinuxDirect(args, drmOutput);
            }

            if (args.Contains("--fbdev"))
                app.StartLinuxFbDev(args);

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

    private class MySplash : LottieSplashToDrm
    {
        public MySplash(DrmOutput drmOutput) : base(drmOutput)
        {
        }

        protected override void Draw(SKCanvas canvas)
        {
            //canvas.DrawImage(SKImage.FromEncodedData("/home/pi/splash.png"), 0, 0);
            // canvas.Flush();
            canvas.Clear(SKColors.White);

            base.Draw(canvas);
        }
    }
}