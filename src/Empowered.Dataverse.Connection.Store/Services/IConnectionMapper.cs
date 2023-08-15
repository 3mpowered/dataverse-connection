using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Store.Services;

internal interface IConnectionMapper
{
    IBaseConnection ToExternal(BaseConnection connection);
    BaseConnection ToInternal(IBaseConnection connection);
}