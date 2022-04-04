using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DumpTruck.Controls;
using DumpTruck.Models;
using DumpTruck.ViewModels;

namespace DumpTruck.Views;

public partial class CarConfigWindow : Window
{
    private const string COLOR = "color";
    
    private const bool WIDE_PALETTE = true;
    
    private CarConfigWindowViewModel _vm;

    private readonly Func<IVehicle, bool>? _callback;

    private string? _receiver;
    
    public CarConfigWindow(Func<IVehicle, bool>? callback = null)
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        _callback = callback;

        var drawArea = this.Get<Drawable>("ModelDrawArea");
        
        _vm = new CarConfigWindowViewModel(drawArea);
        DataContext = _vm;
        
        SetupDnd("SimpleModel", ModelSelected);
        SetupDnd("ExtendedModel", ModelSelected);

        if (WIDE_PALETTE)
        {
            foreach (var i in Enumerable.Range(1, 12))
            {
                SetupDnd("Color" + i, ColorSelected,
                    (data, el) => data.Set(COLOR, (el as Rectangle).Fill.ToString()));
            }
        }
        else
        {
            foreach (var elName in new []{"BodyColor", "TipperColor", "TentColor"})
            {
                foreach (var i in Enumerable.Range(1, 4))
                {
                    SetupDnd(elName + i, ColorSelected,
                        (data, el) => data.Set(COLOR, (el as Rectangle).Fill.ToString()));
                }
            }
        }
        
        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
    }

    public CarConfigWindow()
    {
        // Used by the Designer
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
    
    private void AddClick(object? sender, RoutedEventArgs e)
    {
        var vehicle = _vm.GetVehicle();
        if (_callback != null && vehicle != null)
        {
            var res = _callback.Invoke(vehicle);
            if (res) Close(vehicle);
        }
        else
        {
            Close(vehicle);
        }
    }

    private void CancelClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ModelSelected(string? receiver, DataObject dataObject)
    {
        switch (dataObject.GetText())
        {
            case "SimpleModel":
                _vm.CreateSimpleVehicle();
                break;
            
            case "ExtendedModel":
                _vm.CreateExtendedVehicle();
                break;
        }
    }
    
    private void ColorSelected(string? receiver, DataObject dataObject)
    {
        var color = dataObject.Get(COLOR) as string;
        
        if (receiver == null || color == null) return;
        
        if (receiver.Contains("Body"))
        {
            _vm.BodyColor = color;
        }
        else if (receiver.Contains("Tipper"))
        {
            _vm.TipperColor = color;
        }
        else if (receiver.Contains("Tent"))
        {
            _vm.TentColor = color;
        }
    }
    
    private void SetupDnd(string controlName, Action<string?, DataObject> callback, Action<DataObject, Control>? dataSetter = null)
    {
        var control = this.Get<Control>(controlName);
        if (control == null) return;

        var data = new DataObject();
        data.Set(DataFormats.Text, control.Tag as string ?? control.Name ?? "");
        dataSetter?.Invoke(data, control);
        
        async void DoDrag(object sender, PointerPressedEventArgs e)
        {
            var result = await DragDrop.DoDragDrop(e, data, DragDropEffects.Copy | DragDropEffects.Move);
            switch (result)
            {
                case DragDropEffects.Move:
                case DragDropEffects.Copy:
                    callback(_receiver, data);
                    break;
            }
        }

        control.PointerPressed += DoDrag;
    }
    
    private void DragOver(object sender, DragEventArgs e)
    {
        _validateDragDrop(sender, e);
    }

    private void Drop(object sender, DragEventArgs e)
    {
        _validateDragDrop(sender, e);
    }

    private void _validateDragDrop(object sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.None;
        _receiver = null;
        
        if (e.Source is Control c && e.Data.GetText() != null)
        {
            var receiverName = (c.Name ?? c.Parent?.Name)?.Replace("Receiver", "");
            var senderName = e.Data.GetText();
            
            if (receiverName != null && senderName != null && 
                (senderName.Contains(receiverName) || receiverName.Contains(senderName)))
            {
                e.DragEffects = DragDropEffects.Move; // Copy has non-informational cursor
                _receiver = receiverName;
            }
        }
    }
}
