using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class AzureCliConnection : BaseConnection, IAzureCliConnection
{
    [JsonConstructor]
    public AzureCliConnection()
    {
        Type = ConnectionType.AzureCli;
    }

    [SetsRequiredMembers]
    public AzureCliConnection(IAzureCliConnection connection) : base(connection)
    {
    }

    [SetsRequiredMembers]
    public AzureCliConnection(string name, Uri environmentUrl) : base(name, environmentUrl)
    {
        Type = ConnectionType.AzureCli;
    }

    public override IBaseConnection Clone() => new AzureCliConnection(this);
}