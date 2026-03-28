using System.Windows;
using HardwareStore.Application.Configuration;
using HardwareStore.Infrastructure.Configuration;
using HardwareStore.Infrastructure.Persistence;
using HardwareStore.Desktop.ViewModels;
using HardwareStore.Desktop.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HardwareStore.Desktop;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddApplication()
                    .AddInfrastructure(context.Configuration);

                services.AddSingleton<LoginViewModel>();
                services.AddSingleton<ShellViewModel>();
                services.AddSingleton<LoginWindow>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        await DbInitializer.InitializeAsync(_host.Services);
        await _host.StartAsync();

        var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
