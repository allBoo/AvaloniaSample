using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DumpTruck.ViewModels;
using DumpTruck.Models;

namespace DumpTruck.Views;

public partial class DriveArea : UserControl
{
    private IDrawObject _vehicle;

    public int VehicleSpeed => _vehicle.Speed;
    public float VehicleWeight => _vehicle.Weight;
    public string VehicleBodyColor => _vehicle.BodyColor.ToString();

    public DriveArea()
    {
        InitializeComponent();

        DataContext = new DriveAreaViewModel(this);
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
    
    // Interface

    public void Draw()
    {
        InvalidateVisual();
    }

    public void InitializeVehicle()
    {
        var driveAreaBounds = this.FindControl<Panel>("DriveAreaBounds");
        
        Random rnd = new();

        _vehicle = new Models.DumpTruck(rnd.Next(100, 300), rnd.Next(1000, 2000),
            Color.FromArgb(0xff, (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)));
        _vehicle.SetObject(rnd.Next(10, 100), rnd.Next(10, 100), 
            (int)driveAreaBounds.Bounds.Width, (int)driveAreaBounds.Bounds.Height);

        Trace.WriteLine("Create New Model Handler / Speed " + _vehicle.Speed + " / Weight " +
                        _vehicle.Weight + " / Color " + _vehicle.BodyColor.ToString());
        Draw();
    }
    
    public void Move(string directionStr)
    {
        Trace.WriteLine("MoveCommand " + directionStr);
        
        Enum.TryParse(directionStr, out Direction direction);
        _vehicle.MoveObject(direction);
        Draw();
    }

    public void Resize(Rect newSize)
    {
        Trace.WriteLine("Size changed " + newSize);
        _vehicle?.ChangeBorders((int)newSize.Right, (int)newSize.Bottom);
    }

    // will be called automatically after InvalidateVisual
    public override void Render(DrawingContext context)
    {
        _vehicle?.DrawObject(context);
    }
}
