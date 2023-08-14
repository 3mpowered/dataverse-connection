namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IConnectionWallet
{
    public DateTime TimeStamp { get; }
    IBaseConnection? Current { get; }
    IEnumerable<IBaseConnection> Connections { get; }
}