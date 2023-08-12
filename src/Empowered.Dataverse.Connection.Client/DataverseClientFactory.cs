using Empowered.Dataverse.Connection.Client.Authentication;
using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Client.Settings;
using Empowered.Dataverse.Connection.Store.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;

namespace Empowered.Dataverse.Connection.Client;

public class DataverseClientFactory : IDataverseClientFactory
{
    private readonly IConnectionStore _connectionStore;
    private readonly IConnectionSecretProvider _connectionSecretProvider;
    private readonly IServiceProvider _serviceProvider;

    public DataverseClientFactory(IConnectionStore connectionStore, IConnectionSecretProvider connectionSecretProvider,
        IServiceProvider serviceProvider)
    {
        _connectionStore = connectionStore;
        _connectionSecretProvider = connectionSecretProvider;
        _serviceProvider = serviceProvider;
    }

    public TClient Get<TClient>(string? name) where TClient : class, IOrganizationService
    {
        var memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();
        var connection = string.IsNullOrWhiteSpace(name) ? _connectionStore.GetActive() : _connectionStore.Get(name);
        var connectionSecret = _connectionSecretProvider.GetConnectionSecret(connection.Name);
        var clientOptions = new DataverseClientOptions(connection, connectionSecret);
        var tokenProvider = new TokenProvider(clientOptions, memoryCache);
        var logger = _serviceProvider.GetRequiredService<ILogger<TokenBasedServiceClient>>();

        return new TokenBasedServiceClient(tokenProvider, connection.EnvironmentUrl, logger) as TClient ??
               throw new InvalidCastException($"Client type {typeof(TClient).Name} is invalid.");
    }
}