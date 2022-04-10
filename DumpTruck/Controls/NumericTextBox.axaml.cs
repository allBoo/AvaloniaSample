using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System.Text.RegularExpressions;

namespace DumpTruck.Controls;

public partial class NumericTextBox : TextBox, IStyleable
{
    Type IStyleable.StyleKey => typeof(TextBox);
    
    public NumericTextBox()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        var text = Text ?? string.Empty;
        Text = RemoveNonNumericCharacters(text);
        base.OnKeyDown(e);
    }
    
    protected virtual void OnKeyUp(object? sender, KeyEventArgs e)
    {
        var text = Text ?? string.Empty;
        Text = RemoveNonNumericCharacters(text);
    }

    private string? RemoveNonNumericCharacters(string text)
    {
        const string pattern = @"[^\d]";
        text = Regex.Replace(text, pattern, "");
        
        return text;
    }
}
