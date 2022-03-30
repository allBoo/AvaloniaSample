using System.Threading.Tasks;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace DumpTruck.Helpers;

public static class MessageBox
{
    private const string DefaultTitle = "DumpTruck";

    public static void Show(string message)
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
        {
            ButtonDefinitions = ButtonEnum.Ok,
            ContentTitle = DefaultTitle,
            ContentMessage = message + "   ",
            FontFamily = "Microsoft Sans Serif"
        });
        messageBoxStandardWindow.ShowDialog(App.MainWindow());
    }
    
    public static void ShowError(string message)
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
        {
            ButtonDefinitions = ButtonEnum.Ok,
            ContentTitle = "Error",
            ContentMessage = message + "   ",
            FontFamily = "Microsoft Sans Serif",
            Icon = Icon.Error
        });
        messageBoxStandardWindow.ShowDialog(App.MainWindow());
    }
    
    public static async Task<bool> Confirm(string message, string? title = null)
    {
        var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
        {
            ButtonDefinitions = ButtonEnum.YesNo,
            ContentTitle = title ?? "Confirm",
            ContentMessage = message + "   ",
            FontFamily = "Microsoft Sans Serif",
            Icon = Icon.Question
        });
        var result = await messageBoxStandardWindow.ShowDialog(App.MainWindow());
        return (result & ButtonResult.Yes) != 0;
    }
}
