using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Commands.ConnectionResolvers;

public class UserPasswordConnectionResolver : IConnectionResolver<UserPasswordConnection>
{
    public bool IsApplicable(UpsertConnectionArguments arguments) =>
        !string.IsNullOrWhiteSpace(arguments.ConnectionArguments.TenantId) &&
        !string.IsNullOrWhiteSpace(arguments.ConnectionArguments.Username) &&
        arguments.ConnectionArguments.Password != null;

    public UserPasswordConnection Resolve(UpsertConnectionArguments arguments) =>
        new(
            arguments.ConnectionNameArguments.Name,
            arguments.ConnectionArguments.Url,
            arguments.ConnectionArguments.Username!,
            arguments.ConnectionArguments.Password!.GetPassword(),
            arguments.ConnectionArguments.TenantId!
        );
}