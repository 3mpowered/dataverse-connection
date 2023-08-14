namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IUserPasswordConnection : ITenantConnection
{
    string UserName { get; }
    string Password { get; }
}