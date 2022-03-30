using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DumpTruck.ViewModels;
using DumpTruck.Models;
using DumpTruck.Tests;

namespace DumpTruck.Views;

public partial class DriveArea : UserControl
{
    private IDrawObject _vehicle;

    public int VehicleSpeed => _vehicle.Speed;
    public float VehicleWeight => _vehicle.Weight;
    public string VehicleBodyColor => _vehicle.BodyColor.ToString();

    public DriveArea(IDrawObject vehicle)
    {
        _vehicle = vehicle;
        _vehicle.SetObject(0, 0);
        InitializeComponent();

        DataContext = new DriveAreaViewModel(this);
        
        Draw();
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
    
    // Interface

    private Rect AreaBounds()
    {
        var driveAreaBounds = this.FindControl<Panel>("DriveAreaBounds");
        return driveAreaBounds.Bounds;
    }

    public void Draw()
    {
        InvalidateVisual();
    }

    public void Move(string directionStr)
    {
        Enum.TryParse(directionStr, out Direction direction);
        _vehicle?.MoveObject(direction);
        Draw();
    }

    public void Resize(Rect newSize)
    {
        Trace.WriteLine("Drive Area Size changed " + newSize);
        _vehicle?.ChangeBorders((int)newSize.Right, (int)newSize.Bottom);
    }

    // will be called automatically after InvalidateVisual
    public override void Render(DrawingContext context)
    {
        _vehicle?.DrawObject(context);
    }
    
    /// <summary>
    /// Проведение теста
    /// </summary>
    /// <param name="testObject"></param>
    public void RunTest(AbstractTestObject testObject)
    {
        if (_vehicle == null)
        {
            return;
        }
        
        var bounds = AreaBounds();
        var position = _vehicle.GetCurrentPosition();
        testObject.Init(_vehicle);

        testObject.SetPosition((int)bounds.Width, (int)bounds.Height);
        Helpers.MessageBox.Show(testObject.TestObject());
        
        _vehicle.SetObject(position.Left, position.Top, (int)bounds.Width, (int)bounds.Height);
    }
}
