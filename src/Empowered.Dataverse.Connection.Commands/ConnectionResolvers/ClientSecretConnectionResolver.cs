using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Commands.ConnectionResolvers;

public class ClientSecretConnectionResolver : IConnectionResolver<ClientSecretConnection>
{
    public bool IsApplicable(UpsertConnectionArguments arguments) =>
        !string.IsNullOrWhiteSpace(arguments.ConnectionArguments.TenantId) &&
        !string.IsNullOrWhiteSpace(arguments.ConnectionArguments.ApplicationId) &&
        arguments.ConnectionArguments is { ClientSecret: not null };

    public ClientSecretConnection Resolve(UpsertConnectionArguments arguments) =>
        new(
            arguments.ConnectionNameArguments.Name,
            arguments.ConnectionArguments.Url,
            arguments.ConnectionArguments.ApplicationId!,
            arguments.ConnectionArguments.TenantId!,
            arguments.ConnectionArguments.ClientSecret!.GetPassword()
        );
}