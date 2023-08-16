using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Connection.Store.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spectre.Console;

namespace Empowered.Dataverse.Connection.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectionCommand(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddConnectionStore();
        serviceCollection.AddDataverseClientFactory();

        serviceCollection.TryAddScoped(typeof(ConnectionCommand));
        serviceCollection.TryAddSingleton<IAnsiConsole>(_ => AnsiConsole.Console);
        return serviceCollection;
    }
}