namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IManagedIdentityConnection : IBaseConnection
{
    public string? ClientId { get; }
}