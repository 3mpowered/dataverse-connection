using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class VisualStudioConnection : BaseConnection, IVisualStudioConnection
{
    [JsonConstructor]
    public VisualStudioConnection()
    {
        Type = ConnectionType.VisualStudio;
    }
    
    [SetsRequiredMembers]
    public VisualStudioConnection(IVisualStudioConnection connection) : base(connection)
    {
        
    }
    
    [SetsRequiredMembers]
    public VisualStudioConnection(string name, Uri environmentUrl) : base(name, environmentUrl)
    {
        Type = ConnectionType.VisualStudio;
    }

    public override IBaseConnection Clone() => new VisualStudioConnection(this);
}