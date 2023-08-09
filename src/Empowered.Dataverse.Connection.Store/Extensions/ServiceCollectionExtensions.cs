using Empowered.Dataverse.Connection.Store.Constants;
using Empowered.Dataverse.Connection.Store.Contract;
using Empowered.Dataverse.Connection.Store.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Connection.Store.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectionStore(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<IEnvironmentService>(serviceProvider =>
                new EnvironmentService(
                    serviceProvider.GetRequiredService<ILogger<EnvironmentService>>()
                )
            )
            .AddScoped<IWalletFileService>(serviceProvider =>
                new WalletFileService(
                    serviceProvider.GetRequiredService<IDataProtectionProvider>(),
                    serviceProvider.GetRequiredService<IEnvironmentService>(),
                    serviceProvider.GetRequiredService<ILogger<WalletFileService>>()
                )
            ).AddSingleton<IConnectionStore, ConnectionStore>(serviceProvider => new ConnectionStore(
                    serviceProvider.GetRequiredService<IWalletFileService>(),
                    serviceProvider.GetRequiredService<ILogger<ConnectionStore>>()
                )
            ).AddLogging()
            .AddDataProtection()
            .SetApplicationName(Application.Name);
        return serviceCollection;
    }
}