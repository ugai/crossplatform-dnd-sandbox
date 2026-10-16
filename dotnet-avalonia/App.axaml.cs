using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using dnd.Services;
using dnd.ViewModels;
using dnd.Views;
using Microsoft.Extensions.DependencyInjection;

namespace dnd;

public class App : Application
{
    public new static App? Current => Application.Current as App;

    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // DataAnnotations validation is opt-in since Avalonia 12 (AppBuilder.WithDataAnnotationsValidation),
            // so it no longer has to be removed to avoid duplicate validation with the CommunityToolkit.
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };

            var services = new ServiceCollection();
            services.AddSingleton<FilesService>(x => new FilesService(desktop.MainWindow));
            Services = services.BuildServiceProvider();
        }

        base.OnFrameworkInitializationCompleted();
    }
}