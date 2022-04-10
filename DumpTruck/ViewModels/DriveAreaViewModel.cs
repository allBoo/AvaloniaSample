using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using DumpTruck.Models;
using DumpTruck.Views;
using Image = Avalonia.Controls.Image;

namespace DumpTruck.ViewModels;

public class DriveAreaViewModel : ViewModelBase
{
    public ICommand MoveCommand { get; }

    private readonly DriveArea _parent;
    
    public DriveAreaViewModel(DriveArea parent)
    {
        _parent = parent;
        MoveCommand = ReactiveCommand.Create<string>(MoveModel);
    }

    void MoveModel(string directionStr)
    {
        Trace.WriteLine("MoveCommand " + directionStr);
        
        _parent.Move(directionStr);
    }
}
