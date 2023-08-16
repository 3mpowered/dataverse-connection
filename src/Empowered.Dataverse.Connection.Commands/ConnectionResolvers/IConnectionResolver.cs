using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Commands.ConnectionResolvers;

public interface IConnectionResolver<out TConnection>
    where TConnection : class, IBaseConnection
{
    bool IsApplicable(UpsertConnectionArguments arguments);

    TConnection Resolve(UpsertConnectionArguments arguments);
}