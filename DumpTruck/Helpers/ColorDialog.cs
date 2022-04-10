using System;
using System.Threading.Tasks;
using Avalonia.Media;
using AvaloniaColorPicker;

namespace DumpTruck.Helpers;

public static class ColorDialog
{
    private static readonly Random Rnd = new();
    
    public static ColorPickerWindow ColorWindow(string? title)
    {
        Color RandomColor() =>
            Color.FromArgb(0xff, (byte) Rnd.Next(0, 256), (byte) Rnd.Next(0, 256), (byte) Rnd.Next(0, 256));
        
        var dialog = new ColorPickerWindow
        {
            Title = title ?? "Color picker",
            IsAlphaVisible = false,
            IsHexVisible = false,
            IsHSBVisible = false,
            IsCIELABVisible = false,
            IsPaletteVisible = false,
            IsColourBlindnessSelectorVisible = false,
            IsColourSpaceSelectorVisible = false,
            IsColourSpacePreviewVisible = false,
            Color = RandomColor()
        };
        return dialog;
    }

    public static async Task<Color?> ShowDialog(string? title = null)
    {
        var dialog = ColorWindow(title);
        var color = await dialog.ShowDialog(App.MainWindow());
        return color;
    }
}
