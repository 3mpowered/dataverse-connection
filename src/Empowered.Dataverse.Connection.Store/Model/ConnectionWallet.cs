using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

internal class ConnectionWallet : IConnectionWallet
{
    public ISet<DataverseConnection> ExistingConnections { get; set; } = new HashSet<DataverseConnection>();
    public DataverseConnection? CurrentConnection { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.Now;
    [JsonIgnore] public IDataverseConnection? Current => CurrentConnection?.Clone();
    [JsonIgnore] public IEnumerable<IDataverseConnection> Connections => ExistingConnections.Select(connection => connection.Clone());
}