using confuscsharpnator.UI.WinForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Build.Locator;
using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Infrastructure;
using confuscsharpnator.Core.Models;

namespace confuscsharpnator.Presentation;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Application configuration
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);

        // Dependency injection configuration
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // View
                services.AddSingleton<MainView>();

                services.AddSingleton<IMainView>(provider => provider.GetRequiredService<MainView>());
                services.AddSingleton<IObfuscatorMetadata>(provider => provider.GetRequiredService<MainView>());

                // Presenter
                services.AddSingleton<MainPresenter>();

                // Core services
                services.AddSingleton<IFileWriter, FileWriter>(provider => provider.GetRequiredService<FileWriter>());
                services.AddSingleton<IObfuscationService>(provider => provider.GetRequiredService<ObfuscationService>());
            })
            .Build();

        using var scope = host.Services.CreateScope();
        // Register MSBuild for Roslyn to work properly with .csproj files
        MSBuildLocator.RegisterDefaults();
        var mainPresenter = scope.ServiceProvider.GetRequiredService<MainPresenter>();
        //When the main presenter signals that the view is closed, we want to exit the application.
        mainPresenter.CloseApplication += System.Windows.Forms.Application.Exit;
        mainPresenter.Run();

        System.Windows.Forms.Application.Run();
    }
}
