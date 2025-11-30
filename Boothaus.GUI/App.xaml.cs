using Boothaus.GUI;
using Boothaus.GUI.Services;
using Boothaus.GUI.ViewModels;
using Boothaus.Services.Contracts;
using Boothaus.Services.Persistence;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using DialogService = Boothaus.GUI.Services.DialogService;

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
        services.AddSingleton<IAuftragRepository, AuftragRepository>();
        services.AddSingleton<IBootRepository, BootRepository>();
        services.AddSingleton<ILagerRepository, LagerRepository>();
        services.AddSingleton<SerDes>();
        services.AddSingleton(sp =>
        {
            return new PersistenceService(
                sp.GetRequiredService<SerDes>(),
                Constants.Datenbankpfad,
                sp.GetRequiredService<IAuftragRepository>(),
                sp.GetRequiredService<ILagerRepository>(),
                sp.GetRequiredService<IBootRepository>()
                );
        });

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

        var persistence = Provider.GetRequiredService<PersistenceService>();
        
        persistence.ZustandLaden();

        var appService = Provider.GetRequiredService<LagerApplicationService>();
        var dialogService = Provider.GetRequiredService<IDialogService>();

        var mainWindow = new BoothausMainApplicationWindow(appService, dialogService)
        {
            DataContext = Provider.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }

    void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"Es ist ein unerwarteter Fehler aufgetreten:\n{e.Exception.Message}\nBitte öffnen Sie ein Issue unter github.com/lu-make/boothaus/issues", "Unerwarteter Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
