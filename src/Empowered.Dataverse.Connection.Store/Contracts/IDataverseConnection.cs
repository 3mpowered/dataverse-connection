namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IDataverseConnection : IEquatable<IDataverseConnection>
{
    string Name { get; }
    Uri EnvironmentUrl { get; }
    ConnectionType Type { get; }
    string? ApplicationId { get; }
    string? TenantId { get; }
    string? ClientSecret { get; }
    string? CertificateFilePath { get; }
    string? CertificatePassword { get; }
    string? UserName { get; }
    string? Password { get; }
    
    IDataverseConnection Clone();
    bool IEquatable<IDataverseConnection>.Equals(IDataverseConnection? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }
}