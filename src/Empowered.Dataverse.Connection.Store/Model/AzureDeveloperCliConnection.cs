using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class AzureDeveloperCliConnection : BaseConnection, IAzureDeveloperCliConnection
{
    [JsonConstructor]
    public AzureDeveloperCliConnection()
    {
        Type = ConnectionType.AzureDeveloperCli;
    }

    [SetsRequiredMembers]
    public AzureDeveloperCliConnection(IAzureDeveloperCliConnection connection) : base(connection)
    {

    }
    
    [SetsRequiredMembers]
    public AzureDeveloperCliConnection(string name, Uri environmentUrl) : base(name, environmentUrl)
    {
        Type = ConnectionType.AzureDeveloperCli;
    }

    public override IBaseConnection Clone() => new AzureDeveloperCliConnection(this);
}