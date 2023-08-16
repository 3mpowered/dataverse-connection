using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Commands.Services;

public interface IArgumentConnectionMapper
{
    IBaseConnection ToConnection(UpsertConnectionArguments arguments);
}