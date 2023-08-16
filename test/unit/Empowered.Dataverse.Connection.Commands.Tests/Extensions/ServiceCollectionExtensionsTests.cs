using Empowered.Dataverse.Connection.Commands.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Empowered.Dataverse.Connection.Commands.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldResolveConnectionCommandFromServiceCollection()
    {
        var serviceCollection = new ServiceCollection()
            .AddConnectionCommand();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var connectionCommand = serviceProvider.GetRequiredService<ConnectionCommand>();

        connectionCommand.Should().NotBeNull();
    }
}