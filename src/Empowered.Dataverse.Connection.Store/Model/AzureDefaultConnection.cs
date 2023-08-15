using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class AzureDefaultConnection : BaseConnection, IAzureDefaultConnection
{
    [JsonConstructor]
    public AzureDefaultConnection()
    {
        Type = ConnectionType.AzureDefault;
    }
    
    [SetsRequiredMembers]
    public AzureDefaultConnection(IAzureDefaultConnection connection) : base(connection)
    {
    }

    [SetsRequiredMembers]
    public AzureDefaultConnection(string name, Uri environmentUrl) : base(name, environmentUrl)
    {
        Type = ConnectionType.Interactive;
    }
    
    public override IBaseConnection Clone() => new AzureDefaultConnection(this);
}