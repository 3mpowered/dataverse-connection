namespace Empowered.Dataverse.Connection.Store.Contract;

public interface IConnectionWallet
{
    public DateTime TimeStamp { get; }
    IConnection? Current { get; }
    IEnumerable<IConnection> Connections { get; }
}