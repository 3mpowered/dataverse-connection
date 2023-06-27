namespace Empowered.Dataverse.Connection.Store.Contract;

public interface IConnectionWallet
{
    IConnection? Current { get; }
    IEnumerable<IConnection> Connections { get; }
}