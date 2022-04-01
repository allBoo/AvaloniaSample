using System;
using System.Diagnostics;
using System.Windows.Input;
using DumpTruck.Views;
using ReactiveUI;

namespace DumpTruck.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private GarageArea GarageArea { get; }
        
        private string? _garagePlace;

        public string? GaragePlace
        {
            get => _garagePlace;
            set => this.RaiseAndSetIfChanged(ref _garagePlace, value);
        }
        
        public ICommand ExitCommand { get; }
        public ICommand CreateSimpleCommand { get; }
        public ICommand CreateExtendedCommand { get; }
        public ICommand TakeObjectCommand { get; }
        
        public MainWindowViewModel(GarageArea garageArea)
        {
            GarageArea = garageArea;
            
            ExitCommand = ReactiveCommand.Create(Helpers.App.Exit);
            CreateSimpleCommand = ReactiveCommand.Create(CreateNewSimpleModel);
            CreateExtendedCommand = ReactiveCommand.Create(CreateNewExtendedModel);
            TakeObjectCommand = ReactiveCommand.Create(TakeFromGarage);
        }

        public MainWindowViewModel()
        {
            // used by Designer
        }

        private void CreateNewSimpleModel()
        {
            GarageArea.AddDumpTruck();
        }
        
        private void CreateNewExtendedModel()
        {
            GarageArea.AddTipTruck();
        }

        private void TakeFromGarage()
        {
            if (!string.IsNullOrEmpty(GaragePlace))
            {
                var garagePlaceIdx = Convert.ToInt32(GaragePlace);
                Trace.WriteLine("Take from place " + garagePlaceIdx);
                GarageArea.TakeFromGarage(garagePlaceIdx);
            }
        }
    }
}
