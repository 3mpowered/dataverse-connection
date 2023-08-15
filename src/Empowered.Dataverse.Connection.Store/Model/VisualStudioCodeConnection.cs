using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class VisualStudioCodeConnection : BaseConnection, IVisualStudioCodeConnection
{
    [JsonConstructor]
    public VisualStudioCodeConnection()
    {
        Type = ConnectionType.VisualStudioCode;
    }

    [SetsRequiredMembers]
    public VisualStudioCodeConnection(IVisualStudioCodeConnection connection) : base(connection)
    {
    }

    [SetsRequiredMembers]
    public VisualStudioCodeConnection(string name, Uri environmentUrl) : base(name, environmentUrl)

    {
        Type = ConnectionType.VisualStudioCode;
    }

    public override IBaseConnection Clone() => new VisualStudioCodeConnection(this);
}