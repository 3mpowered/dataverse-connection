using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Empowered.Dataverse.Connection.Store.Factories;

public static class ConnectionSecretProviderFactory
{
    public static IConnectionSecretProvider Get()
    {
        var provider = new ServiceCollection()
            .AddConnectionSecretProvider()
            .BuildServiceProvider();

        return provider.GetRequiredService<IConnectionSecretProvider>();
    }
}