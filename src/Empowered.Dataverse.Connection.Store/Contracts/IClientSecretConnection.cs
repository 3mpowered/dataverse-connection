namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IClientSecretConnection : IApplicationConnection
{
    string ClientSecret { get; }
}