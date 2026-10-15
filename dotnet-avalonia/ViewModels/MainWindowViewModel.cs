using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using dnd.Services;
using Microsoft.Extensions.DependencyInjection;

namespace dnd.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<string> Paths { get; } = [];

    [RelayCommand]
    private async Task FileDialogRequest()
    {
        if (App.Current?.Services == null)
            return;

        var filesService = App.Current.Services.GetRequiredService<FilesService>();
        var files = await filesService.PickFilesAsync();
        SetPaths(files);
    }

    [RelayCommand]
    private void Drop(IEnumerable<IStorageItem> files)
    {
        SetPaths(files);
    }

    // Keep the previous list when nothing was picked or dropped
    private void SetPaths(IEnumerable<IStorageItem> items)
    {
        // IStorageItem.Path is a file:// URI; show the local path when there is one
        var paths = items.Select(f => f.TryGetLocalPath() ?? f.Path.ToString()).ToList();
        if (paths.Count == 0)
            return;

        Paths.Clear();
        foreach (var path in paths)
            Paths.Add(path);
    }
}