using System.Collections.Generic;
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
    [ObservableProperty] private string _path = "";

    [RelayCommand]
    private async Task FileDialogRequest()
    {
        if (App.Current?.Services == null)
            return;

        var filesService = App.Current.Services.GetRequiredService<FilesService>();
        var files = await filesService.PickFilesAsync();
        var paths = string.Join(", ", files.Select(f => f.Path));
        Path = paths;
    }

    [RelayCommand]
    private void Drop(IEnumerable<IStorageItem> files)
    {
        var paths = string.Join(", ", files.Select(f => f.Path));
        Path = paths;
    }
}