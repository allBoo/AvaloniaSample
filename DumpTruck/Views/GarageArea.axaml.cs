using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using DumpTruck.Models;

    
namespace DumpTruck.Views;

public partial class GarageArea : UserControl
{
    private Garage<IVehicle>? _garage;
    
    public GarageArea()
    {
        InitializeComponent();
        Draw();
    }

    public void SetGarage(Garage<IVehicle>? garage)
    {
        _garage = garage;
        
        Resize(AreaBounds());
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private Rect AreaBounds()
    {
        var driveAreaBounds = this.FindControl<Panel>("GarageAreaBounds");
        return driveAreaBounds.Bounds;
    }

    // events
    private void GarageAreaBoundsChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
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
        Trace.WriteLine("Garage Area Size Changed " + newSize);
        _garage?.Resize((int)newSize.Width, (int)newSize.Height);
        Draw();
    }

    public override void Render(DrawingContext context)
    {
        _garage?.Draw(context);
    }
    
    // interface

    public async void AddDumpTruck()
    {
        if (_garage == null) return;
        
        var color = await Helpers.ColorDialog.ShowDialog("Цвет кузова");
        if (color != null)
        {
            Random rnd = new();
            var vehicle = new Models.DumpTruck(rnd.Next(100, 300), rnd.Next(1000, 2000), (Color)color);
            AddToGarage(vehicle);
        }
    }
    
    public async void AddTipTruck()
    {
        if (_garage == null) return;
        
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
                    AddToGarage(vehicle);
                }
            }
            
        }
    }

    public void AddToGarage(IVehicle vehicle)
    {
        if (_garage == null) return;
        
        Trace.WriteLine("Add '" + vehicle.GetType().Name + "' Car / Speed " + vehicle.Speed + 
                        " / Weight " + vehicle.Weight + " / Color " + vehicle.BodyColor);

        try
        {
            _ = _garage + vehicle;
        }
        catch (OverflowException e)
        {
            Helpers.MessageBox.ShowError(e.Message);
        }
    }

    public void TakeFromGarage(int index)
    {
        if (_garage == null) return;

        try
        {
            var car = _garage - index;
            var driveWindow = new DriveWindow(car);
            Helpers.App.ShowDialog(driveWindow);
            Draw();
        }
        catch (IndexOutOfRangeException e) 
        {
            Helpers.MessageBox.ShowError(e.Message);            
        }
    }
}
