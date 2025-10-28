using Avalonia;
using System;
using System.IO;

namespace WowFontManager;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            // Log error to file for debugging
            var errorLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
            try
            {
                var errorMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Application Startup Error\n" +
                                 $"Message: {ex.Message}\n" +
                                 $"Type: {ex.GetType().FullName}\n" +
                                 $"Stack Trace:\n{ex.StackTrace}\n" +
                                 $"\nInner Exception: {ex.InnerException?.ToString() ?? "None"}\n" +
                                 new string('=', 80) + "\n\n";
                
                File.AppendAllText(errorLog, errorMessage);
                Console.WriteLine($"FATAL ERROR: Application failed to start. Error logged to: {errorLog}");
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch
            {
                Console.WriteLine($"FATAL ERROR: {ex}");
            }
            
            Environment.Exit(1);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
