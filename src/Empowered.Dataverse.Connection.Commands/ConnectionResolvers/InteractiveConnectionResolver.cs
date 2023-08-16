using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Commands.ConnectionResolvers;

public class InteractiveConnectionResolver : IConnectionResolver<InteractiveConnection>
{
    public bool IsApplicable(UpsertConnectionArguments arguments) => arguments.ConnectionArguments.Interactive;

    public InteractiveConnection Resolve(UpsertConnectionArguments arguments) =>
        new(arguments.ConnectionNameArguments.Name, arguments.ConnectionArguments.Url);
}