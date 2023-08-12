namespace Empowered.Dataverse.Connection.Store.Model;

public class PublicConnection : AbstractConnection
{
    public PublicConnection(string name, Uri environmentUrl)
    {
        Name = name;
        EnvironmentUrl = environmentUrl;
    }

    public sealed override string Name { get; init; }
    public sealed override Uri EnvironmentUrl { get; init; }
    public override string? TenantId { get; init; }
    public override string? ApplicationId { get; init; }
    public override string? CertificateFilePath { get; init; }
    public override string? UserName { get; init; }
}