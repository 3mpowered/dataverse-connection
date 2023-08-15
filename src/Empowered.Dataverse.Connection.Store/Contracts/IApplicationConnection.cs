namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IApplicationConnection : ITenantConnection
{
    string ApplicationId { get; }
}