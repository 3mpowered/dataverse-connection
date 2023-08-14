using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

internal class ConnectionWallet : IConnectionWallet
{
    public ISet<BaseConnection> ExistingConnections { get; set; } = new HashSet<BaseConnection>();
    public BaseConnection? CurrentConnection { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.Now;
    [JsonIgnore] public IBaseConnection? Current => CurrentConnection?.Clone();
    [JsonIgnore] public IEnumerable<IBaseConnection> Connections => ExistingConnections.Select(connection => connection.Clone());
}