using Empowered.Dataverse.Connection.Store.Contract;
using Empowered.Dataverse.Connection.Store.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Empowered.Dataverse.Connection.Store.Extensions;

public class ServiceCollectionExtensionTests
{
    [Fact]
    public void ShouldInstantiateConnectionStoreFromServiceProvider()
    {
        var collection = new ServiceCollection()
            .AddConnectionStore();

        var serviceProvider = collection.BuildServiceProvider();

        var connectionStore = serviceProvider.GetRequiredService<IConnectionStore>();

        connectionStore.Should().NotBeNull();
        connectionStore.Should().BeOfType<ConnectionStore>();
    }

    [Fact]
    public void ShouldInstantiateConnectionSecretProviderFromServiceProvider()
    {
        var collection = new ServiceCollection().AddConnectionSecretProvider();

        var provider = collection.BuildServiceProvider();

        var connectionSecretProvider = provider.GetRequiredService<IConnectionSecretProvider>();

        connectionSecretProvider.Should().NotBeNull();
        connectionSecretProvider.Should().BeOfType<ConnectionSecretProvider>();
    }
}