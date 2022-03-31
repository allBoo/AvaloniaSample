using System;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DumpTruck.Controls;
using DumpTruck.ViewModels;

namespace DumpTruck.Views;

public partial class CarConfigWindow : Window
{
    private const string COLOR = "color";
    
    private CarConfigWindowViewModel _vm;
    public CarConfigWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        var drawArea = this.Get<Drawable>("ModelDrawArea");
        
        _vm = new CarConfigWindowViewModel(drawArea);
        DataContext = _vm;
        
        SetupDnd("SimpleModel", ModelSelected);
        SetupDnd("ExtendedModel", ModelSelected);

        foreach (var elName in new []{"BodyColor", "TipperColor", "TentColor"})
        {
            foreach (var i in Enumerable.Range(1, 4))
            {
                SetupDnd(elName + i, ColorSelected,
                    (data, el) => data.Set(COLOR, (el as Rectangle).Fill.ToString()));
            }
        }
        
        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
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
    
    private void CancelClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ModelSelected(DataObject dataObject)
    {
        Trace.WriteLine("Selected Model " + dataObject.GetText());
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
    
    private void ColorSelected(DataObject dataObject)
    {
        var part = dataObject.GetText();
        var color = dataObject.Get(COLOR) as string;
        Trace.WriteLine("Selected Color " + part + " / " + color);
        
        if (part == null || color == null) return;
        
        if (part.Contains("Body"))
        {
            Trace.WriteLine("Update Body Color " + color);
            _vm.BodyColor = color;
        }
        else if (part.Contains("Tipper"))
        {
            _vm.TipperColor = color;
        }
        else if (part.Contains("Tent"))
        {
            _vm.TentColor = color;
        }
    }
    
    private void SetupDnd(string controlName, Action<DataObject> callback, Action<DataObject, Control>? dataSetter = null)
    {
        var control = this.Get<Control>(controlName);
        if (control == null) return;

        var data = new DataObject();
        data.Set(DataFormats.Text, control.Name ?? "");
        dataSetter?.Invoke(data, control);
        
        async void DoDrag(object sender, PointerPressedEventArgs e)
        {
            var result = await DragDrop.DoDragDrop(e, data, DragDropEffects.Copy | DragDropEffects.Move);
            switch (result)
            {
                case DragDropEffects.Move:
                case DragDropEffects.Copy:
                    callback(data);
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
        if (e.Source is Control c && e.Data.GetText() != null)
        {
            var controlName = c.Name ?? c.Parent?.Name;
            if (controlName != null && e.Data.GetText().Contains(controlName.Replace("Receiver", "")))
            {
                e.DragEffects = DragDropEffects.Move; // Copy has non-informational cursor
            }
        }
    }
}
