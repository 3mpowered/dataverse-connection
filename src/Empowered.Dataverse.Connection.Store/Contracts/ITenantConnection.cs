namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface ITenantConnection : IBaseConnection
{
    string TenantId { get; }
}