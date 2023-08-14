using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class ClientCertificateConnection : ApplicationConnection, IClientCertificateConnection
{
    [JsonConstructor]
    public ClientCertificateConnection()
    {
        Type = ConnectionType.ClientCertificate;
    }
    [SetsRequiredMembers]
    public ClientCertificateConnection(IClientCertificateConnection connection) : base(connection)
    {
        FilePath = connection.FilePath;
        Password = connection.Password;
    }

    [SetsRequiredMembers]
    public ClientCertificateConnection(string name, Uri environmentUrl, string applicationId, string tenantId, string filePath, string password)
        : base(name, environmentUrl, applicationId, tenantId)
    {
        Type = ConnectionType.ClientCertificate;
        FilePath = filePath;
        Password = password;
    }

    public required string FilePath { get; init; }
    public required string Password { get; init; }

    public override IBaseConnection Clone() => new ClientCertificateConnection(this);
}