using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DumpTruck.ViewModels;

namespace DumpTruck.Views;

public partial class DriveWindow : Window
{
    public DriveWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        
        // create interaction area for the model
        DriveArea area = new DriveArea();
            
        // search DriveArea container and insert DriveArea control into it
        var driveAreaPanel = this.FindControl<Panel>("DriveArea");
        driveAreaPanel.Children.Add(area);

        // create window view-model and pass DriveArea into it, so Window can interact with it
        DataContext = new DriveWindowViewModel(area);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}