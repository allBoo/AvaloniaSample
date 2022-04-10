using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using DumpTruck.ViewModels;

namespace DumpTruck.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _vm;
        
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            // search DriveArea container and insert DriveArea control into it
            var garage = this.FindControl<GarageArea>("GarageArea");

            // create window view-model and pass DriveArea into it, so Window can interact with it
            _vm = new MainWindowViewModel(garage);
            DataContext = _vm;
        }

        private void TakeCarOnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                _vm.TakeObjectCommand.Execute(null);
            }
        }
    }
}
