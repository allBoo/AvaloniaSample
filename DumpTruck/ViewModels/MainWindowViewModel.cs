using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using DumpTruck.Views;

namespace DumpTruck.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DriveArea _area { get; }

        public int Speed => _area.VehicleSpeed;
        public float Weight => _area.VehicleWeight;
        public string BodyColor => "#" + _area.VehicleBodyColor;
        
        
        public ICommand CreateCommand { get; }
        public ICommand ExitCommand { get; }
        
        public MainWindowViewModel(DriveArea area)
        {
            _area = area;

            CreateCommand = ReactiveCommand.Create(CreateNewModel);
            
            ExitCommand = ReactiveCommand.Create(() =>
            {
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
                {
                    Trace.TraceWarning("Exit");
                    lifetime.Shutdown();
                }
            });
        }

        public MainWindowViewModel()
        {
            // used by Designer
        }

        void CreateNewModel()
        {
            _area.InitializeVehicle();

            this.RaisePropertyChanged(nameof(Speed));
            this.RaisePropertyChanged(nameof(Weight));
            this.RaisePropertyChanged(nameof(BodyColor));
        }
    }
}
