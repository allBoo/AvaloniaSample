using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using DumpTruck.Tests;
using ReactiveUI;
using DumpTruck.Views;

namespace DumpTruck.ViewModels
{
    public class DriveWindowViewModel : ViewModelBase
    {
        private DriveArea _area { get; }

        public int Speed => _area.VehicleSpeed;
        public float Weight => _area.VehicleWeight;
        public string BodyColor => _area.VehicleBodyColor;
        public ICommand CloseCommand { get; }
        
        public DriveWindowViewModel(DriveArea area)
        {
            _area = area;

            CloseCommand = ReactiveCommand.Create(() =>
            {
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                {
                    Trace.WriteLine("Exit");
                    lifetime.Shutdown();
                }
            });
        }

        public DriveWindowViewModel()
        {
            // used by Designer
        }

        private void _updateStatusBar()
        {
            this.RaisePropertyChanged(nameof(Speed));
            this.RaisePropertyChanged(nameof(Weight));
            this.RaisePropertyChanged(nameof(BodyColor));
        }
    }
}
