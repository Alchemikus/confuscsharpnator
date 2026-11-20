using confuscsharpnator.Core.Abstractions;
using confuscsharpnator.Core.Infrastructure;
using confuscsharpnator.Core.Models;
using confuscsharpnator.Core.Passes;
using confuscsharpnator.Shared;
using confuscsharpnator.UI.WinForms;
using Microsoft.Build.Locator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

                // Core obfuscation features
                var coreAssembly = typeof(IObfuscatorStep).Assembly;

                var stepTypes = coreAssembly.GetTypes()
                    .Where(t => typeof(IObfuscatorStep).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

                foreach (var type in stepTypes)
                {
                    // Register class
                    services.AddSingleton(type);

                    // Register interfaces
                    services.AddSingleton(typeof(IObfuscatorStep), provider => provider.GetRequiredService(type));
                    services.AddSingleton(typeof(IObfuscationFeatureInfo), provider => provider.GetRequiredService(type));
                }

                // Core services
                services.AddSingleton<IFileWriter, FileWriter>();
                services.AddSingleton<IObfuscationService, ObfuscationService>();
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
