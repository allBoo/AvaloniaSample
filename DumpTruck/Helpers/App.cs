using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Diagnostics;
using Avalonia.Controls;

namespace DumpTruck.Helpers;

public static class App
{
    public static Window MainWindow()
    {
        var desktop = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return desktop.MainWindow;
    }
    
    public static void Exit()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            Trace.WriteLine("Exit");
            lifetime.Shutdown();
        }
    }

    public static void ShowDialog(Window window)
    {
        window.ShowDialog(MainWindow());
    }
}
