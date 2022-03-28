using Avalonia.Controls;
using DumpTruck.ViewModels;

namespace DumpTruck.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // create interaction area for the model
            DriveArea area = new DriveArea();
            
            // search DriveArea container and insert DriveArea control into it
            var driveAreaPanel = this.FindControl<Panel>("DriveArea");
            driveAreaPanel.Children.Add(area);

            // create window view-model and pass DriveArea into it, so Window can interact with it
            DataContext = new MainWindowViewModel(area);
        }
    }
}