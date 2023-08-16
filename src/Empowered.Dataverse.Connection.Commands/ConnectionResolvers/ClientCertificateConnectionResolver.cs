using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Commands.ConnectionResolvers;

public class ClientCertificateConnectionResolver : IConnectionResolver<ClientCertificateConnection>
{
    public bool IsApplicable(UpsertConnectionArguments arguments) =>
        !string.IsNullOrWhiteSpace(arguments.ConnectionArguments.TenantId) &&
        !string.IsNullOrWhiteSpace(arguments.ConnectionArguments.ApplicationId) &&
        arguments.ConnectionArguments is { CertificatePassword: not null, CertificateFilePath: not null };

    public ClientCertificateConnection Resolve(UpsertConnectionArguments arguments) =>
        new(
            arguments.ConnectionNameArguments.Name,
            arguments.ConnectionArguments.Url,
            arguments.ConnectionArguments.ApplicationId!,
            arguments.ConnectionArguments.TenantId!,
            arguments.ConnectionArguments.CertificateFilePath!.FullName,
            arguments.ConnectionArguments.CertificatePassword!.GetPassword()
        );
}