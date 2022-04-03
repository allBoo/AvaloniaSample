using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using DumpTruck.Models;
using NLog;
    
namespace DumpTruck.Views;

public partial class GarageArea : UserControl
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    
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
        logger.Debug("Garage Area Size Changed " + newSize);
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
        
        try
        {
            _ = _garage + vehicle;
            Draw();
            
            logger.Info($"New car {vehicle} has added to the garage {_garage.Name}");
        }
        catch (OverflowException e)
        {
            logger.Warn($"Unable to add car to the garage {_garage.Name} with error {e.Message}");
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
            
            logger.Info($"Car {car} has beem taken from grarage {_garage.Name}/{index}");
        }
        catch (IndexOutOfRangeException e) 
        {
            logger.Warn($"Unable to take car from garage {_garage.Name}/{index} with error {e.Message}");
            Helpers.MessageBox.ShowError(e.Message);
        }
    }
}
