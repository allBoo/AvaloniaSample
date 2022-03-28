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
        private DriveArea _area { get; }

        public int Speed => _area.VehicleSpeed;
        public float Weight => _area.VehicleWeight;
        public string BodyColor => _area.VehicleBodyColor;
        
        private int? _testIndex;
        public int? TestIndex
        {
            get => _testIndex;
            set => this.RaiseAndSetIfChanged(ref _testIndex, value);
        }

        public ICommand CreateSimpleCommand { get; }
        public ICommand CreateExtendedCommand { get; }
        public ICommand RunTestCommand { get; }
        public ICommand ExitCommand { get; }
        
        public MainWindowViewModel(DriveArea area)
        {
            _area = area;

            CreateSimpleCommand = ReactiveCommand.Create(CreateNewSimpleModel);
            CreateExtendedCommand = ReactiveCommand.Create(CreateNewExtendedModel);
            RunTestCommand = ReactiveCommand.Create(RunTest);
            
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

        void RunTest()
        {
            Trace.WriteLine("Run Test #" + TestIndex);
            switch(TestIndex)
            {
                case 0:
                    _area.RunTest(new BordersTestObject());
                    break;
            }
        }
    }
}
