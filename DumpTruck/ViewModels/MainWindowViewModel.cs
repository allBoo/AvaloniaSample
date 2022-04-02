using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Input;
using Avalonia.Controls;
using DumpTruck.Models;
using DumpTruck.Views;
using ReactiveUI;

namespace DumpTruck.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly GarageCollection _garageCollection;
        private GarageArea GarageArea { get; }
        
        private string? _garagePlace;
        public string? GaragePlace
        {
            get => _garagePlace;
            set => this.RaiseAndSetIfChanged(ref _garagePlace, value);
        }

        private string? _newGarageName;
        public string? NewGarageName
        {
            get => _newGarageName;
            set => this.RaiseAndSetIfChanged(ref _newGarageName, value);
        }

        private int? _selectedGarageIndex;
        public int? SelectedGarageIndex
        {
            get => _selectedGarageIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedGarageIndex, value);
                GarageChangedCommand.Execute(value);
                this.RaisePropertyChanged(nameof(IsGarageActive));
            }
        }
        
        public bool? IsGarageActive => SelectedGarageIndex > -1;

        public ObservableCollection<string> GarageItems => new (_garageCollection.Keys);

        public ICommand ExitCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand AddVehicleCommand { get; }
        public ICommand TakeObjectCommand { get; }
        public ICommand CreateGarageCommand { get; }
        public ICommand DeleteGarageCommand { get; }
        public ICommand GarageChangedCommand { get; }

        // flag to prevent handling of the GarageChanged event
        private bool _lockChanges = false;
        
        public MainWindowViewModel(GarageArea garageArea)
        {
            GarageArea = garageArea;
            _garageCollection = new GarageCollection(0, 0);
            
            ExitCommand = ReactiveCommand.Create(Helpers.App.Exit);
            OpenFileCommand = ReactiveCommand.Create(OpenFile);
            SaveFileCommand = ReactiveCommand.Create(SaveFile);
            
            AddVehicleCommand = ReactiveCommand.Create(AddVehicle);
            TakeObjectCommand = ReactiveCommand.Create(TakeFromGarage);
            CreateGarageCommand = ReactiveCommand.Create(CreateNewGarage);
            DeleteGarageCommand = ReactiveCommand.Create<int>(DeleteGarage);
            GarageChangedCommand = ReactiveCommand.Create<int>(GarageChanged);
        }

        public MainWindowViewModel()
        {
            // used by Designer
        }

        private void ReloadLevels()
        {
            _lockChanges = true;
            this.RaisePropertyChanged(nameof(GarageItems));
            _lockChanges = false;
            
            if (SelectedGarageIndex is null or -1 && _garageCollection.Count > 0)
            {
                SelectedGarageIndex = 0;
            }
            else if (_garageCollection.Count == 0)
            {
                SelectedGarageIndex = -1;                
            }
        }

        private void CreateNewSimpleModel()
        {
            GarageArea.AddDumpTruck();
        }
        
        private void CreateNewExtendedModel()
        {
            GarageArea.AddTipTruck();
        }

        private async void AddVehicle()
        {
            // option 1. Use ShowDialog
            /*
            var configWindow = new CarConfigWindow();
            var result = await configWindow.ShowDialog<IVehicle?>(Helpers.App.MainWindow());
            if (result != null)
            {
                _addVehicle(result);
            }
            */
            
            // Option 2. Use Callback
            var configWindow = new CarConfigWindow(_addVehicle);
            configWindow.Show(Helpers.App.MainWindow());
        }

        private void _addVehicle(IVehicle vehicle)
        {
            Trace.WriteLine("Got New Vehicle " + vehicle);
            GarageArea.AddToGarage(vehicle);
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

        private void CreateNewGarage()
        {
            if (!string.IsNullOrEmpty(NewGarageName))
            {
                if (!_garageCollection.AddGarage(NewGarageName))
                {
                    Helpers.MessageBox.ShowError("Гараж с таким названием уже существует");
                }

                ReloadLevels();
            }
        }
        
        private async void DeleteGarage(int index)
        {
            if (index > -1 && await Helpers.MessageBox.Confirm("Удалить гараж?"))
            {
                Trace.WriteLine("Delete Garage " + index + " / " + GarageItems[index]);
                if (_garageCollection.DelGarage(GarageItems[index]))
                {
                    ReloadLevels();
                }
                else
                {
                    Helpers.MessageBox.ShowError("Нет такого гаража");
                }
            }
        }

        private void GarageChanged(int index)
        {
            if (_lockChanges) return;
            
            if (index > -1)
            {
                Trace.WriteLine("Garage Changed to " + index + " / " + GarageItems[index]);
                GarageArea.SetGarage(_garageCollection[GarageItems[index]]);
            }
            else
            {
                Trace.WriteLine("Garage Unselected");
                GarageArea.SetGarage(null);
            }
        }

        private void OpenFile()
        {
        }

        private async void SaveFile()
        {
            var dialog = new SaveFileDialog()
            {
                DefaultExtension = ".txt",
                // Directory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                InitialFileName = "DumpTruck.txt",
                Filters = new List<FileDialogFilter>
                {
                    new(){Name = "Text files (.txt)", Extensions = new List<string>{"txt"}}
                },
                Title = "Сохранить гараж"
            };
            var fileName = await dialog.ShowAsync(Helpers.App.MainWindow());
            if (fileName == null) return;

            if (System.IO.File.Exists(fileName) && 
                !await Helpers.MessageBox.Confirm($"Перезаписать файл {fileName}?"))
            {
                return;
            }

            try
            {
                using (var file = new StreamWriter(fileName, false, new UTF8Encoding(true)))
                {
                    _garageCollection.DumpToFile(file);
                }
            }
            catch (IOException e)
            {
                Helpers.MessageBox.ShowError($"Unable to save to file {fileName} with error {e}");
            }
        }
    }
}
