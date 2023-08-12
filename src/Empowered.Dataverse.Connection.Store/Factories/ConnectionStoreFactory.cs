using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Empowered.Dataverse.Connection.Store.Factories;

public static class ConnectionStoreFactory
{
    public static IConnectionStore Get()
    {
        var serviceProvider = new ServiceCollection()
            .AddConnectionStore()
            .BuildServiceProvider();
        return serviceProvider.GetRequiredService<IConnectionStore>();
    }
}