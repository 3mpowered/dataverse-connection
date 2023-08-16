using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Commands.ConnectionResolvers;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Commands.Services;

public class ArgumentConnectionMapper : IArgumentConnectionMapper
{
    private readonly IEnumerable<IConnectionResolver<IBaseConnection>> _resolvers;

    public ArgumentConnectionMapper(IEnumerable<IConnectionResolver<IBaseConnection>> resolvers)
    {
        _resolvers = resolvers;
    }

    public IBaseConnection ToConnection(UpsertConnectionArguments arguments) => _resolvers
        .Single(resolver => resolver.IsApplicable(arguments))
        .Resolve(arguments);
}