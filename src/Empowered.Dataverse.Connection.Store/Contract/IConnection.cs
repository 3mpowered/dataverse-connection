namespace Empowered.Dataverse.Connection.Store.Contract;

public interface IConnection : IEquatable<IConnection>
{
    ConnectionType ConnectionType { get; }
    string Name { get; }
    Uri EnvironmentUrl { get; }
    string? TenantId { get; }
    string? ApplicationId { get; }
    public string? CertificateFilePath { get; }
    string? UserName { get; }

    bool IEquatable<IConnection>.Equals(IConnection? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }
}