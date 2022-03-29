using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using DumpTruck.Tests;
using ReactiveUI;
using DumpTruck.Views;

namespace DumpTruck.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand CreateSimpleCommand { get; }
        public ICommand CreateExtendedCommand { get; }
        public ICommand ExitCommand { get; }
        
        public MainWindowViewModel()
        {
            CreateSimpleCommand = ReactiveCommand.Create(CreateNewSimpleModel);
            CreateExtendedCommand = ReactiveCommand.Create(CreateNewExtendedModel);
            
            ExitCommand = ReactiveCommand.Create(() =>
            {
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                {
                    Trace.WriteLine("Exit");
                    lifetime.Shutdown();
                }
            });
        }

        // public MainWindowViewModel()
        // {
        //     // used by Designer
        // }

        void CreateNewSimpleModel()
        {
            // _area.InitializeVehicle();
        }
        
        void CreateNewExtendedModel()
        {
            // _area.InitializeVehicle(true);
        }
    }
}
