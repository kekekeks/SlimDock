using System;
using Avalonia;
namespace Sandbox
{
    class Program
    {
        public static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new X11PlatformOptions()
                {
                    //UseGpu = false,
                    UseDeferredRendering = false
                })
                .LogToTrace();
    }
}