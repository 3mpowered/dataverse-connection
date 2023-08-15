using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class AzurePowershellConnection : BaseConnection, IAzurePowershellConnection
{
    [JsonConstructor]
    public AzurePowershellConnection()
    {
        Type = ConnectionType.AzurePowershell;
    }
    
    [SetsRequiredMembers]
    public AzurePowershellConnection(IAzurePowershellConnection connection) : base(connection)
    {
        
    }
    
    [SetsRequiredMembers]
    public AzurePowershellConnection(string name, Uri environmentUrl) : base(name, environmentUrl)
    {
        Type = ConnectionType.AzurePowershell;
    }

    public override IBaseConnection Clone() => new AzurePowershellConnection(this);
}