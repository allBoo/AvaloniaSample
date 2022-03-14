using System;
using System.Diagnostics;
using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DumpTruck.ViewModels;
using DumpTruck.Models;
using Color = System.Drawing.Color;

namespace DumpTruck.Views;

public partial class DriveArea : UserControl
{
    private readonly Models.DumpTruck _vehicle;

    public int VehicleSpeed => _vehicle.Speed;
    public float VehicleWeight => _vehicle.Weight;
    public string VehicleBodyColor => _vehicle.BodyColor.Name;

    public DriveArea(Models.DumpTruck vehicle)
    {
        InitializeComponent();

        _vehicle = vehicle;

        DataContext = new DriveAreaViewModel(this);
    }

    public DriveArea()
    {
        // used by Designer
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // events

    private void DrawArea_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == "Bounds" && e.NewValue != null)
        {
            Resize((Rect) e.NewValue);
        }
    }
    
    private void MoveButtonClick(object? sender, RoutedEventArgs e)
    {
        //
        Trace.WriteLine("MoveButtonClick Sender " + ((Button)sender).ToString());
        Trace.WriteLine("MoveButtonClick " + e.Route);
    }

    // Interface

    public void Draw()
    {
        InvalidateVisual();
    }

    public void InitializeVehicle()
    {
        Random rnd = new();

        _vehicle.Init(rnd.Next(100, 300), rnd.Next(1000, 2000),
            Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256)));
        _vehicle.SetPosition(rnd.Next(10, 100), rnd.Next(10, 100));

        Trace.WriteLine("Create New Model Handler / Speed " + _vehicle.Speed + " / Weight " +
                        _vehicle.Weight + " / Color " + _vehicle.BodyColor.Name);
        Draw();
    }
    
    public void Move(string directionStr)
    {
        Trace.WriteLine("MoveCommand " + directionStr);
        
        Enum.TryParse(directionStr, out Direction direction);
        _vehicle.MoveTransport(direction);
        Draw();
    }

    public void Resize(Rect newSize)
    {
        Trace.WriteLine("Size changed " + newSize);
        _vehicle.ChangeBorders((int)newSize.Right, (int)newSize.Bottom);
    }

    // will be called automatically after InvalidateVisual
    public override void Render(DrawingContext context)
    {
        Trace.WriteLine("Render");
        _vehicle.Draw(context);
    }
}
