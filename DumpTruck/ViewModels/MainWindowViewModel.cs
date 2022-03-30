using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using DumpTruck.Models;
using DumpTruck.Views;
using ReactiveUI;

namespace DumpTruck.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ParkingCollection _parkingCollection;
        private ParkingArea ParkingArea { get; }
        
        private string? _parkingPlace;
        public string? ParkingPlace
        {
            get => _parkingPlace;
            set => this.RaiseAndSetIfChanged(ref _parkingPlace, value);
        }

        private string? _newParkingName;
        public string? NewParkingName
        {
            get => _newParkingName;
            set => this.RaiseAndSetIfChanged(ref _newParkingName, value);
        }

        private int? _selectedParkingIndex;
        public int? SelectedParkingIndex
        {
            get => _selectedParkingIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedParkingIndex, value);
                ParkingChangedCommand.Execute(value);
                this.RaisePropertyChanged(nameof(IsParkingActive));
            }
        }
        
        public bool? IsParkingActive => SelectedParkingIndex > -1;

        public ObservableCollection<string> ParkingItems => new (_parkingCollection.Keys);

        public ICommand ExitCommand { get; }
        public ICommand CreateSimpleCommand { get; }
        public ICommand CreateExtendedCommand { get; }
        public ICommand TakeObjectCommand { get; }
        public ICommand CreateParkingCommand { get; }
        public ICommand DeleteParkingCommand { get; }
        public ICommand ParkingChangedCommand { get; }

        // flag to prevent handling of the ParkingChanged event
        private bool _lockChanges = false;
        
        public MainWindowViewModel(ParkingArea parkingArea)
        {
            ParkingArea = parkingArea;
            _parkingCollection = new ParkingCollection(0, 0);
            
            ExitCommand = ReactiveCommand.Create(Helpers.App.Exit);
            CreateSimpleCommand = ReactiveCommand.Create(CreateNewSimpleModel);
            CreateExtendedCommand = ReactiveCommand.Create(CreateNewExtendedModel);
            TakeObjectCommand = ReactiveCommand.Create(TakeFromParking);
            CreateParkingCommand = ReactiveCommand.Create(CreateNewParking);
            DeleteParkingCommand = ReactiveCommand.Create<int>(DeleteParking);
            ParkingChangedCommand = ReactiveCommand.Create<int>(ParkingChanged);
        }

        public MainWindowViewModel()
        {
            // used by Designer
        }

        private void ReloadLevels()
        {
            _lockChanges = true;
            this.RaisePropertyChanged(nameof(ParkingItems));
            _lockChanges = false;
            
            if (SelectedParkingIndex is null or -1 && _parkingCollection.Count > 0)
            {
                SelectedParkingIndex = 0;
            }
            else if (_parkingCollection.Count == 0)
            {
                SelectedParkingIndex = -1;                
            }
        }

        private void CreateNewSimpleModel()
        {
            ParkingArea.AddDumpTruck();
        }
        
        private void CreateNewExtendedModel()
        {
            ParkingArea.AddTipTruck();
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

        private void CreateNewParking()
        {
            if (!string.IsNullOrEmpty(NewParkingName))
            {
                if (!_parkingCollection.AddParking(NewParkingName))
                {
                    Helpers.MessageBox.ShowError("Парковка с таким названием уже существует");
                }

                ReloadLevels();
            }
        }
        
        private void DeleteParking(int index)
        {
            if (index > -1)
            {
                Trace.WriteLine("Delete parking " + index + " / " + ParkingItems[index]);
                if (_parkingCollection.DelParking(ParkingItems[index]))
                {
                    ReloadLevels();
                }
                else
                {
                    Helpers.MessageBox.ShowError("Нет такой парковки");
                }
            }
        }

        private void ParkingChanged(int index)
        {
            if (_lockChanges) return;
            
            if (index > -1)
            {
                Trace.WriteLine("Parking Changed to " + index + " / " + ParkingItems[index]);
                ParkingArea.SetParking(_parkingCollection[ParkingItems[index]]);
            }
            else
            {
                Trace.WriteLine("Parking Unselected");
                ParkingArea.SetParking(null);
            }
        }
    }
}
