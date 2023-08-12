using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

internal class ConnectionWallet : IConnectionWallet
{
    public ISet<SecretConnection> ExistingConnections { get; set; } = new HashSet<SecretConnection>();
    public SecretConnection? CurrentConnection { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.Now;
    [JsonIgnore] public IConnection? Current => CurrentConnection?.ToPublicConnection();
    [JsonIgnore] public IEnumerable<IConnection> Connections => ExistingConnections.Select(connection => connection.ToPublicConnection());
}