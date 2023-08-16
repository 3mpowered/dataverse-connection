using System.Reflection;
using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Connection.Commands.ConnectionResolvers;
using Empowered.Dataverse.Connection.Commands.Services;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Empowered.Dataverse.Connection.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectionCommand(this IServiceCollection serviceCollection)
    {
        
        // Add all classes impementing IConnectionResolver<TCommand> to service collection
        var resolverInterface = typeof(IConnectionResolver<IBaseConnection>);
        var resolvers = Assembly.GetAssembly(resolverInterface)!
            .GetTypes()
            .Where(type => resolverInterface.IsAssignableFrom(type) &&
                           type is
                           {
                               IsClass: true, 
                               IsAbstract: false, 
                               IsInterface: false
                           }
            );
        foreach (var resolver in resolvers)
        {
            serviceCollection.AddTransient(resolverInterface, resolver);
        }

        serviceCollection.TryAddScoped<IArgumentConnectionMapper, ArgumentConnectionMapper>();
        serviceCollection.TryAddScoped<ConnectionCommand>();
        return serviceCollection
            .AddConnectionStore()
            .AddDataverseClientFactory();
    }
}