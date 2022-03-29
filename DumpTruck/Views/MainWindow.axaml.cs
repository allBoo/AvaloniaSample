using Avalonia.Controls;
using DumpTruck.ViewModels;

namespace DumpTruck.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // search DriveArea container and insert DriveArea control into it
            var parking = this.FindControl<ParkingArea>("ParkingArea");

            // create window view-model and pass DriveArea into it, so Window can interact with it
            DataContext = new MainWindowViewModel(parking);
        }
    }
}
