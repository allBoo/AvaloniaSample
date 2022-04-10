using System.Diagnostics;
using Avalonia.Media;
using DumpTruck.Controls;
using DumpTruck.Models;
using ReactiveUI;

namespace DumpTruck.ViewModels;

public class CarConfigWindowViewModel : ViewModelBase
{
    private IVehicle? _vehicle;

    private int _maxSpeed = 100;
    public int MaxSpeed
    {
        get => _maxSpeed;
        set
        {
            this.RaiseAndSetIfChanged(ref _maxSpeed, value);
            if (_vehicle != null) _vehicle.Speed = value;
        }
    }

    private int _weight = 1000;
    public int Weight
    {
        get => _weight;
        set
        {
            this.RaiseAndSetIfChanged(ref _weight, value);
            if (_vehicle != null) _vehicle.Weight = value;
        }
    }

    public bool HasVehicle => _vehicle != null;
    
    public bool IsExtended => _vehicle is TipTruck;

    public bool HasTipper
    {
        get => _vehicle is TipTruck {Tipper: true};
        set
        {
            if (_vehicle is TipTruck truck) truck.Tipper = value;
            this.RaisePropertyChanged(nameof(HasTipper));
            Draw();
        }
    }

    public bool HasTent
    {
        get => _vehicle is TipTruck {Tent: true};
        set
        {
            if (_vehicle is TipTruck truck) truck.Tent = value;
            this.RaisePropertyChanged(nameof(HasTent));
            Draw();
        }
    }

    public string? BodyColor
    {
        get => _vehicle?.BodyColor.ToString() ?? Brushes.Transparent.Color.ToString();
        set
        {
            if (_vehicle != null && value != null) _vehicle.BodyColor = Color.Parse(value);
            this.RaisePropertyChanged(nameof(BodyColor));
            Draw();
        }
    }
    
    public string? TipperColor
    {
        get => _vehicle is TipTruck truck ? truck.TipperColor.ToString() : Brushes.Transparent.Color.ToString();
        set
        {
            if (_vehicle is TipTruck truck && value != null) truck.TipperColor = Color.Parse(value);
            this.RaisePropertyChanged(nameof(TipperColor));
            Draw();
        }
    }
    
    public string? TentColor
    {
        get => _vehicle is TipTruck truck ? truck.TentColor.ToString() : Brushes.Transparent.Color.ToString();
        set
        {
            if (_vehicle is TipTruck truck && value != null) truck.TentColor = Color.Parse(value);
            this.RaisePropertyChanged(nameof(TentColor));
            Draw();
        }
    }

    private Drawable _drawArea;

    public CarConfigWindowViewModel(Drawable drawArea)
    {
        _drawArea = drawArea;
    }

    public CarConfigWindowViewModel()
    {
        // Used by the Designer
    }

    private void Draw()
    {
        _drawArea.Draw();
    }

    public IVehicle? GetVehicle()
    {
        return _vehicle;
    }

    public void CreateSimpleVehicle()
    {
        SetVehicle(new Models.DumpTruck(MaxSpeed, Weight, Brushes.White.Color));
    }

    public void CreateExtendedVehicle()
    {
        SetVehicle(new TipTruck(MaxSpeed, Weight, Brushes.White.Color, 
            true, Brushes.Black.Color, true, Brushes.LightBlue.Color));
    }

    private void SetVehicle(IVehicle vehicle)
    {
        _vehicle = vehicle;
        _drawArea.SetObject(_vehicle);
        
        this.RaisePropertyChanged(nameof(HasVehicle));
        this.RaisePropertyChanged(nameof(IsExtended));
        this.RaisePropertyChanged(nameof(HasTipper));
        this.RaisePropertyChanged(nameof(HasTent));
        this.RaisePropertyChanged(nameof(BodyColor));
        this.RaisePropertyChanged(nameof(TipperColor));
        this.RaisePropertyChanged(nameof(TentColor));
        Draw();
    }
}
