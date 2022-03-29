using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using System.Diagnostics;
using DumpTruck.Models;

    
namespace DumpTruck.Views;

public partial class ParkingArea : UserControl
{
    private readonly Parking<IDrawObject> _parking;
    
    public ParkingArea()
    {
        InitializeComponent();

        var bounds = AreaBounds();
        Trace.WriteLine("Initial parking bounds " + bounds);
        _parking = new Parking<IDrawObject>((int)bounds.Width, (int)bounds.Height);
        Draw();
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
        _parking.Resize((int)newSize.Width, (int)newSize.Height);
        Draw();
    }

    public override void Render(DrawingContext context)
    {
        Trace.WriteLine("Parking Area Render");
        _parking.Draw(context);
    }
}
