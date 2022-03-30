using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using DumpTruck.Models;
using DumpTruck.ViewModels;

namespace DumpTruck.Views;

public partial class DriveWindow : Window
{
    public DriveWindow(IDrawObject vehicle)
    {
        InitializeComponent();
        
        // create interaction area for the model
        DriveArea area = new DriveArea(vehicle);
        
        // search DriveArea container and insert DriveArea control into it
        var driveAreaPanel = this.FindControl<Panel>("DriveArea");
        driveAreaPanel.Children.Add(area);

        // create window view-model and pass DriveArea into it, so Window can interact with it
        DataContext = new DriveWindowViewModel(area);
    }

    public DriveWindow()
    {
        // used by Designer
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void WindowOnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }
}
