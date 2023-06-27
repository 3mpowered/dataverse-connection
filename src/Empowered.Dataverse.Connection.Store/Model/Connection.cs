using System.Security;
using Empowered.Dataverse.Connection.Store.Contract;

namespace Empowered.Dataverse.Connection.Store.Model;

internal class Connection : IConnection
{
    public ConnectionType ConnectionType { get; }
    public string Name { get; }
    public Uri EnvironmentUrl { get; }
    public string TenantId { get; }
    public string ApplicationId { get; }
    public SecureString ClientSecret { get; init; }
    public string CertificateFilePath { get; init; }
    public SecureString CertificatePassword { get; init; }
    public string UserName { get; }
    public SecureString Password { get; init; }
}