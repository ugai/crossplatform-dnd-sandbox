using Avalonia.Controls;
using Avalonia.Input;
using dnd.ViewModels;

namespace dnd.Views;

public partial class MainWindow : Window
{
    private const string DragOverClass = "drag-over";

    public MainWindow()
    {
        InitializeComponent();
        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DragLeaveEvent, DragLeave);
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        var hasFiles = e.Data.Contains(DataFormats.Files);
        e.DragEffects = hasFiles ? DragDropEffects.Copy : DragDropEffects.None;
        SetDragOver(hasFiles);
    }

    private void DragLeave(object? sender, DragEventArgs e)
    {
        SetDragOver(false);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        SetDragOver(false);
        var files = e.Data.GetFiles();
        if (files != null && DataContext is MainWindowViewModel vm)
            vm.DropCommand.Execute(files);
    }

    private void SetDragOver(bool isDragOver)
    {
        if (isDragOver && !DropArea.Classes.Contains(DragOverClass))
            DropArea.Classes.Add(DragOverClass);
        else if (!isDragOver)
            DropArea.Classes.Remove(DragOverClass);
    }
}
