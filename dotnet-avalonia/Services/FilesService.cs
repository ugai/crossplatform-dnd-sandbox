using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace dnd.Services;

public class FilesService(Window target)
{
    public async Task<IReadOnlyList<IStorageFile>> PickFilesAsync()
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(target);

        // Start async operation to open the dialog.
        if (topLevel == null)
            return new List<IStorageFile>();

        var opts = new FilePickerOpenOptions
        {
            Title = "Open File",
            AllowMultiple = true
        };
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(opts);
        return files;
    }
}