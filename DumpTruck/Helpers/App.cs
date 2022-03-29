using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Diagnostics;

namespace DumpTruck.Helpers;

public static class App
{
    public static void Exit()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            Trace.WriteLine("Exit");
            lifetime.Shutdown();
        }
    }
}
