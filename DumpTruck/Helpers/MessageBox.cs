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
}
