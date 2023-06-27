using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contract;

namespace Empowered.Dataverse.Connection.Store.Model;

internal class ConnectionWallet : IConnectionWallet
{
    public readonly ISet<Connection> ExistingConnections = new HashSet<Connection>();
    public Connection? CurrentConnection;
    [JsonIgnore] public IConnection? Current => CurrentConnection;
    [JsonIgnore] public IEnumerable<IConnection> Connections => ExistingConnections;
}