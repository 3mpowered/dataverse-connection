using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Commands.ConnectionResolvers;

public class DeviceCodeConnectionResolver : IConnectionResolver<DeviceCodeConnection>
{
    public bool IsApplicable(UpsertConnectionArguments arguments) => arguments.ConnectionArguments.DeviceCode;

    public DeviceCodeConnection Resolve(UpsertConnectionArguments arguments) =>
        new(arguments.ConnectionNameArguments.Name, arguments.ConnectionArguments.Url);
}