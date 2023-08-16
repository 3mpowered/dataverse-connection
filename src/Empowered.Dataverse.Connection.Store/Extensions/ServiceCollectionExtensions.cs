using Empowered.Dataverse.Connection.Store.Constants;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Connection.Store.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectionStore(this IServiceCollection serviceCollection)
    {
        TryAddCommonDependencies(serviceCollection);

        serviceCollection
            .AddSingleton<IConnectionStore, ConnectionStore>(serviceProvider => new ConnectionStore(
                    serviceProvider.GetRequiredService<IWalletFileService>(),
                    serviceProvider.GetRequiredService<ILogger<ConnectionStore>>()
                )
            );

        return serviceCollection;
    }
    
    private static void TryAddCommonDependencies(IServiceCollection serviceCollection)
    {
        serviceCollection
            .TryAddTransient<IEnvironmentService>(serviceProvider =>
                new EnvironmentService(
                    serviceProvider.GetRequiredService<ILogger<EnvironmentService>>()
                )
            );
        serviceCollection.TryAddScoped<IWalletFileService>(serviceProvider =>
            new WalletFileService(
                serviceProvider.GetRequiredService<IDataProtectionProvider>(),
                serviceProvider.GetRequiredService<IEnvironmentService>(),
                serviceProvider.GetRequiredService<ILogger<WalletFileService>>()
            )
        );
        
        serviceCollection
            .AddLogging()
            .AddDataProtection()
            .SetApplicationName(Application.Name);
    }
}