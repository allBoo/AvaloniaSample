using Avalonia.Controls;
using DumpTruck.ViewModels;

namespace DumpTruck.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // create window view-model and pass DriveArea into it, so Window can interact with it
            DataContext = new MainWindowViewModel();
        }
    }
}