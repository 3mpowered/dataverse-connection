using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public abstract class TenantConnection : BaseConnection, ITenantConnection
{
    [JsonConstructor]
    public TenantConnection()
    {
        Type = ConnectionType.Unknown;
    }
    
    [SetsRequiredMembers]
    protected TenantConnection(ITenantConnection connection) : base(connection)
    {
        TenantId = connection.TenantId;
    }

    [SetsRequiredMembers]
    protected TenantConnection(string name, Uri environmentUrl, string tenantId) : base(name, environmentUrl)
    {
        TenantId = tenantId;
    }

    public required string TenantId { get; init; }
}