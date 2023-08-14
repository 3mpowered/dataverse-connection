namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IClientCertificateConnection : IApplicationConnection
{
    string FilePath { get; }
    string Password { get; }
}