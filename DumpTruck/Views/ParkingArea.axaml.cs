using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using DumpTruck.Models;

    
namespace DumpTruck.Views;

public partial class ParkingArea : UserControl
{
    private Parking<IVehicle>? _parking;
    
    public ParkingArea()
    {
        InitializeComponent();
        Draw();
    }

    public void SetParking(Parking<IVehicle>? parking)
    {
        _parking = parking;
        
        Resize(AreaBounds());
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private Rect AreaBounds()
    {
        var driveAreaBounds = this.FindControl<Panel>("ParkingAreaBounds");
        return driveAreaBounds.Bounds;
    }

    // events
    private void ParkingAreaBoundsChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == "Bounds" && e.NewValue != null)
        {
            Resize((Rect) e.NewValue);
        }
    }
    
    public void Draw()
    {
        InvalidateVisual();
    }
    
    public void Resize(Rect newSize)
    {
        Trace.WriteLine("Parking Area Size Changed " + newSize);
        _parking?.Resize((int)newSize.Width, (int)newSize.Height);
        Draw();
    }

    public override void Render(DrawingContext context)
    {
        _parking?.Draw(context);
    }
    
    // interface

    public async void AddDumpTruck()
    {
        if (_parking == null) return;
        
        var color = await Helpers.ColorDialog.ShowDialog("Цвет кузова");
        if (color != null)
        {
            Random rnd = new();
            var vehicle = new Models.DumpTruck(rnd.Next(100, 300), rnd.Next(1000, 2000), (Color)color);
            AddToParking(vehicle);
        }
    }
    
    public async void AddTipTruck()
    {
        if (_parking == null) return;
        
        var bodyColor = await Helpers.ColorDialog.ShowDialog("Цвет кузова");
        if (bodyColor != null)
        {
            var tipperColor = await Helpers.ColorDialog.ShowDialog("Цвет самосвала");
            if (tipperColor != null)
            {
                var tentColor = await Helpers.ColorDialog.ShowDialog("Цвет тента");
                if (tentColor != null)
                {
                    Random rnd = new();
                    var vehicle = new Models.TipTruck(rnd.Next(100, 300), rnd.Next(1000, 2000), (Color)bodyColor, 
                        true, (Color)tipperColor, true, (Color)tentColor);
                    AddToParking(vehicle);
                }
            }
            
        }
    }

    private void AddToParking(IVehicle vehicle)
    {
        if (_parking == null) return;
        
        Trace.WriteLine("Add '" + vehicle.GetType().Name + "' Car / Speed " + vehicle.Speed + 
                        " / Weight " + vehicle.Weight + " / Color " + vehicle.BodyColor);

        if (_parking + vehicle)
        {
            Draw();
        }
        else
        {
            Helpers.MessageBox.Show("Парковка переполнена");
        }
    }

    public void TakeFromParking(int index)
    {
        if (_parking == null) return;
        
        var car = _parking - index;
        if (car != null)
        {
            // 
            var driveWindow = new DriveWindow(car);
            Helpers.App.ShowDialog(driveWindow);
            Draw();
        }
        else
        {
            Helpers.MessageBox.Show("Парковочное место пусто");
        }
    }
}
