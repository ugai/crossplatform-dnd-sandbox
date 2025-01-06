using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using dnd.ViewModels;

namespace dnd.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var files = e.Data.GetFiles();
        if (files != null)
            ((MainWindowViewModel)DataContext!).DropCommand.Execute(files);
    }
}