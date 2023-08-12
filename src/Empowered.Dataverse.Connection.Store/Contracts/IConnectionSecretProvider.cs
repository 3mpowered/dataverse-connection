namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IConnectionSecretProvider
{
    string GetConnectionSecret(string name);
}