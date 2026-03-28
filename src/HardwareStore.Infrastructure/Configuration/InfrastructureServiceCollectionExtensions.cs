using HardwareStore.Application.Abstractions;
using HardwareStore.Application.Interfaces;
using HardwareStore.Infrastructure.Options;
using HardwareStore.Infrastructure.Persistence;
using HardwareStore.Infrastructure.Security;
using HardwareStore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HardwareStore.Infrastructure.Configuration;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

        var storageOptions = configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>() ?? new StorageOptions();
        var dataDirectory = ResolvePath(storageOptions.DataDirectory);
        Directory.CreateDirectory(dataDirectory);
        Directory.CreateDirectory(ResolvePath(storageOptions.BackupDirectory));
        Directory.CreateDirectory(ResolvePath(storageOptions.LogDirectory));
        Directory.CreateDirectory(ResolvePath(storageOptions.ExportDirectory));

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            var databasePath = Path.Combine(dataDirectory, storageOptions.DatabaseFileName);
            connectionString = $"Data Source={databasePath}";
        }

        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IBackupService, BackupService>();
        services.AddScoped<IExportService, CsvExportService>();

        return services;
    }

    private static string ResolvePath(string relativeOrAbsolute)
    {
        if (Path.IsPathRooted(relativeOrAbsolute))
        {
            return relativeOrAbsolute;
        }

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativeOrAbsolute));
    }
}
