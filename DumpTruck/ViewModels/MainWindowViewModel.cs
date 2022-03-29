using System;
using System.Diagnostics;
using System.Windows.Input;
using DumpTruck.Views;
using ReactiveUI;

namespace DumpTruck.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ParkingArea ParkingArea { get; }
        
        private string? _parkingPlace;

        public string? ParkingPlace
        {
            get => _parkingPlace;
            set => this.RaiseAndSetIfChanged(ref _parkingPlace, value);
        }
        
        public ICommand ExitCommand { get; }
        public ICommand CreateSimpleCommand { get; }
        public ICommand CreateExtendedCommand { get; }
        public ICommand TakeObjectCommand { get; }
        
        public MainWindowViewModel(ParkingArea parkingArea)
        {
            ParkingArea = parkingArea;
            
            ExitCommand = ReactiveCommand.Create(Helpers.App.Exit);
            CreateSimpleCommand = ReactiveCommand.Create(CreateNewSimpleModel);
            CreateExtendedCommand = ReactiveCommand.Create(CreateNewExtendedModel);
            TakeObjectCommand = ReactiveCommand.Create(TakeFromParking);
        }

        public MainWindowViewModel()
        {
            // used by Designer
        }

        private void CreateNewSimpleModel()
        {
            ParkingArea.AddVehicle();
        }
        
        private void CreateNewExtendedModel()
        {
            ParkingArea.AddVehicle(true);
        }

        private void TakeFromParking()
        {
            if (!string.IsNullOrEmpty(ParkingPlace))
            {
                var parkingPlaceIdx = Convert.ToInt32(ParkingPlace);
                Trace.WriteLine("Take from place " + parkingPlaceIdx);
                ParkingArea.TakeFromParking(parkingPlaceIdx);
            }
        }
    }
}
