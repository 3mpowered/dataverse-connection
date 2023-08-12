using CommandDotNet;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Tool.Arguments;

public class UpsertConnectionArguments : IArgumentModel
{
    public required ConnectionNameArguments ConnectionNameArguments { get; set; }
    public required ConnectionArguments ConnectionArguments { get; set; }

    public PublicConnection ToConnection()
    {
        return new PublicConnection(ConnectionNameArguments.Name, new Uri(ConnectionArguments.Url))
        {
            ApplicationId = ConnectionArguments.ClientCredentials?.ApplicationId,
            TenantId = ConnectionArguments.ClientCredentials?.TenantId,
            UserName = ConnectionArguments.UserCredentials?.Username,
            CertificateFilePath = ConnectionArguments.ClientCredentials?.CertificateFilePath?.FullName,
        };
    }
}