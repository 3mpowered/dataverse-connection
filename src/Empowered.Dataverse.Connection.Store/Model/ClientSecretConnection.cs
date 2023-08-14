using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class ClientSecretConnection : ApplicationConnection, IClientSecretConnection
{
    [JsonConstructor]
    public ClientSecretConnection()
    {
        Type = ConnectionType.ClientSecret;
    }
    
    [SetsRequiredMembers]
    public ClientSecretConnection(IClientSecretConnection connection) : base(connection)
    {
        ClientSecret = connection.ClientSecret;
    }

    [SetsRequiredMembers]
    public ClientSecretConnection(string name, Uri environmentUrl, string applicationId, string tenantId, string clientSecret) : base(name,
        environmentUrl, applicationId,
        tenantId)
    {
        Type = ConnectionType.ClientSecret;
        ClientSecret = clientSecret;
    }

    public required string ClientSecret { get; init; }

    public override IBaseConnection Clone() => new ClientSecretConnection(this);
}