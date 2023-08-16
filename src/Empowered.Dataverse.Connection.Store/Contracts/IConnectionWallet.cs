namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IConnectionWallet
{
    public DateTime TimeStamp { get; }
    IDataverseConnection? Current { get; }
    IEnumerable<IDataverseConnection> Connections { get; }
}