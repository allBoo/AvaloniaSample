using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DumpTruck.Models;

namespace DumpTruck.Controls;

public partial class Drawable : Border
{
    private IDrawable? _drawable;
    
    public Drawable()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    public void SetObject(IDrawable drawable)
    {
        _drawable = drawable;

        var dimensions = drawable.GetDimensions();
        var x = ((float)Bounds.Width - dimensions.Width) / 2;
        var y = ((float)Bounds.Height - dimensions.Height) / 2;
        _drawable.SetObject(x, y);
        Draw();
    }

    public void Draw()
    {
        InvalidateVisual();
    }
    
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        _drawable?.DrawObject(context);
    }
}
