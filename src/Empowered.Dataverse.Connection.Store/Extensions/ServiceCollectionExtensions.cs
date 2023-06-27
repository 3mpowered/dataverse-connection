using Empowered.Dataverse.Connection.Store.Constants;
using Empowered.Dataverse.Connection.Store.Contract;
using Empowered.Dataverse.Connection.Store.Services;
using Empowered.Dataverse.Connection.Store.Store;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace Empowered.Dataverse.Connection.Store.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectionStore(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IConnectionStore, ConnectionStore>()
            .AddTransient<IWalletFileService, WalletFileService>()
            .AddDataProtection()
            .SetApplicationName(Application.Name);
        return serviceCollection;
    }
}