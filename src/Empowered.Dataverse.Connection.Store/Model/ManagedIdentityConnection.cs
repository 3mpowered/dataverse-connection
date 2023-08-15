using System.Diagnostics.CodeAnalysis;
using Empowered.Dataverse.Connection.Store.Contracts;
using Newtonsoft.Json;

namespace Empowered.Dataverse.Connection.Store.Model;

public class ManagedIdentityConnection : BaseConnection, IManagedIdentityConnection
{
    [JsonConstructor]
    public ManagedIdentityConnection()
    {
        Type = ConnectionType.ManagedIdentity;
    }

    [SetsRequiredMembers]
    public ManagedIdentityConnection(IManagedIdentityConnection connection) : base(connection)
    {
        ClientId = connection.ClientId;
    }

    [SetsRequiredMembers]
    public ManagedIdentityConnection(string name, Uri environmentUrl, string? clientId = null) : base(name, environmentUrl)
    {
        Type = ConnectionType.ManagedIdentity;
        ClientId = clientId;
    }

    public override IBaseConnection Clone() => new ManagedIdentityConnection(this);
    
    public string? ClientId { get; init; }
}