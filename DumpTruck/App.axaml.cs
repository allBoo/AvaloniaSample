using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DumpTruck.ViewModels;
using DumpTruck.Views;
using System;
using System.Diagnostics;

namespace DumpTruck
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;
            Trace.Indent();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // desktop.MainWindow = new MainWindow();
                desktop.MainWindow = new CarConfigWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}