using CommandDotNet;
using Empowered.Dataverse.Connection.Tool.Constants;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Empowered.Dataverse.Connection.Tool.Arguments;

public class ConnectionArguments : IArgumentModel
{
    [Option(Description = "A flag to interactively authenticate against the Dataverse environment")]
    public bool Interactive { get; init; }

    [Option(Description = "A flag to interactively authenticate via device code against the Dataverse environment")]
    public bool DeviceCode { get; init; }

    [Option(Description = "The URL of the Dataverse environment")]
    [EnvVar(ConfigurationKeys.EnvironmentUrl)]
    public required Uri Url { get; init; }

    [Option(Description = "The username to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.Username)]
    public string? Username { get; init; }

    [Option(Description = "The passwort to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.Password)]
    public Password? Password { get; init; }

    [Option(Description = "The application id to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.ApplicationId)]
    public string? ApplicationId { get; init; }

    [Option(Description = "The client secret to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.ClientSecret)]
    public Password? ClientSecret { get; init; }

    [Option(Description = "The tenant id to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.TenantId)]
    public string? TenantId { get; init; }

    [Option(Description = "The absolute path to the certificate to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.CertificateFilePath)]
    public FileInfo? CertificateFilePath { get; init; }

    [Option(Description = "The certificate password to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.CertificatePassword)]
    public Password? CertificatePassword { get; init; }
}