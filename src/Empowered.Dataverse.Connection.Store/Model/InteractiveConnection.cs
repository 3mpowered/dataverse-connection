using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class InteractiveConnection : BaseConnection, IInteractiveConnection
{
    [JsonConstructor]
    public InteractiveConnection()
    {
        Type = ConnectionType.Interactive;
    }
    
    [SetsRequiredMembers]
    public InteractiveConnection(IInteractiveConnection connection) : base(connection)
    {
    }

    [SetsRequiredMembers]
    public InteractiveConnection(string name, Uri environmentUrl) : base(name, environmentUrl)
    {
        Type = ConnectionType.Interactive;
    }


    public override IBaseConnection Clone() => new InteractiveConnection(this);
}