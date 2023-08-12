using CommandDotNet;
using Empowered.Dataverse.Connection.Tool.Constants;

namespace Empowered.Dataverse.Connection.Tool.Arguments;

public class ClientCredentials : IArgumentModel
{
    [Option(Description = "The application id to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.ApplicationId)]
    public string? ApplicationId { get; set; }

    [Option(Description = "The client secret to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.ClientSecret)]
    public Password? ClientSecret { get; set; }

    [Option(Description = "The tenant id to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.TenantId)]
    public string? TenantId { get; set; }

    [Option(Description = "The absolute path to the certificate to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.CertificateFilePath)]
    public FileInfo? CertificateFilePath { get; set; }

    [Option(Description = "The certificate password to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.CertificatePassword)]
    public Password? CertificatePassword { get; set; }
}