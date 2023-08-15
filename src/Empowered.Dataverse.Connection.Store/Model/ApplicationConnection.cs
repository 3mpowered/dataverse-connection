using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public abstract class ApplicationConnection : TenantConnection, IApplicationConnection
{
    [JsonConstructor]
    protected ApplicationConnection()
    {
    }
    
    [SetsRequiredMembers]
    protected ApplicationConnection(IApplicationConnection connection) : base(connection)
    {
        ApplicationId = connection.ApplicationId;
    }

    [SetsRequiredMembers]
    protected ApplicationConnection(string name, Uri environmentUrl, string applicationId, string tenantId) : base(name, environmentUrl, tenantId)
    {
        ApplicationId = applicationId;
    }

    public required string ApplicationId { get; init; }
}