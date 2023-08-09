using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contract;

namespace Empowered.Dataverse.Connection.Store.Model;

internal class ConnectionWallet : IConnectionWallet
{
    public ConnectionWallet()
    {
        TimeStamp = DateTime.Now;
    }

    public ISet<Connection> ExistingConnections { get; set; } = new HashSet<Connection>();
    public Connection? CurrentConnection { get; set; }
    public DateTime TimeStamp { get; set; }
    [JsonIgnore] public IConnection? Current => CurrentConnection;
    [JsonIgnore] public IEnumerable<IConnection> Connections => ExistingConnections;
}