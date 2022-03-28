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
        public string BodyColor => _area.VehicleBodyColor;
        
        
        public ICommand CreateSimpleCommand { get; }
        public ICommand CreateExtendedCommand { get; }
        public ICommand ExitCommand { get; }
        
        public MainWindowViewModel(DriveArea area)
        {
            _area = area;

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

        public MainWindowViewModel()
        {
            // used by Designer
        }

        void CreateNewSimpleModel()
        {
            _area.InitializeVehicle();

            _updateStatusBar();
        }
        
        void CreateNewExtendedModel()
        {
            _area.InitializeVehicle(true);

            _updateStatusBar();
        }
        
        private void _updateStatusBar()
        {
            this.RaisePropertyChanged(nameof(Speed));
            this.RaisePropertyChanged(nameof(Weight));
            this.RaisePropertyChanged(nameof(BodyColor));
        }
    }
}
