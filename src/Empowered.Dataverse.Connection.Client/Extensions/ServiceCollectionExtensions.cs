using Empowered.Dataverse.Connection.Client.Authentication;
using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Client.Settings;
using Empowered.Dataverse.Connection.Store.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Connection.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataverseClient<TClient>(this IServiceCollection serviceCollection)
        where TClient : class, IOrganizationService
    {
        serviceCollection.AddRequiredDependencies();
        serviceCollection.TryAddScoped(typeof(TClient), typeof(TokenBasedServiceClient));
        return serviceCollection;
    }

    public static IServiceCollection AddDataverseClientFactory(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddRequiredDependencies()
            .AddConnectionStore()
            .AddConnectionSecretProvider();
        serviceCollection.TryAddSingleton<IDataverseClientFactory, DataverseClientFactory>();
        return serviceCollection;
    }

    private static IServiceCollection AddRequiredDependencies(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddMemoryCache()
            .AddOptions<DataverseClientOptions>()
            .Configure<IConfiguration>((options, configuration) => configuration
                .GetSection(DataverseClientOptions.Section)
                .Bind(options)
            );
        serviceCollection.TryAddScoped<ITokenProvider, TokenProvider>();
        return serviceCollection;
    }
}