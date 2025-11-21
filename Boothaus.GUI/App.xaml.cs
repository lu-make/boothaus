using Boothaus.GUI;
using Boothaus.GUI.Services;
using Boothaus.GUI.ViewModels;
using Boothaus.Services.Contracts;
using Boothaus.Services.Persistence;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Boothaus;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Provider = services.BuildServiceProvider();
    }

    public ServiceProvider Provider { get; private set; }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IAuftragRepository, InMemoryAuftragRepository>();
        services.AddSingleton<IBootRepository, InMemoryBootRepository>();
        services.AddSingleton<ILagerRepository, InMemoryLagerRepository>();
        services.AddSingleton<ImportExportService>();
        services.AddSingleton<LagerApplicationService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddTransient<MainViewModel>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var auftragRepo = Provider.GetRequiredService<IAuftragRepository>();
        var bootRepo = Provider.GetRequiredService<IBootRepository>();
        var lagerRepo = Provider.GetRequiredService<ILagerRepository>();
        
        bootRepo.InitialisiereMitDefaults(DefaultData.Boote());
        lagerRepo.InitialisiereMitDefaults(DefaultData.Lager());
        auftragRepo.InitialisiereMitDefaults(DefaultData.Aufträge(lagerRepo.GetLager(), bootRepo.GetAll()));

        var mainWindow = new BoothausMainApplicationWindow
        {
            DataContext = Provider.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }

    void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        System.Windows.MessageBox.Show($"Es ist ein unerwarteter Fehler aufgetreten:\n{e.Exception.Message}\nBitte öffnen Sie ein Issue unter github.com/lu-make/boothaus/issues", "AAAA", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
