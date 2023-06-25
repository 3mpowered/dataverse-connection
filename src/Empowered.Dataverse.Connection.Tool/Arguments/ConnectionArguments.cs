using CommandDotNet;
using Empowered.Dataverse.Connection.Tool.Constants;

namespace Empowered.Dataverse.Connection.Tool.Arguments;

public class ConnectionArguments : IArgumentModel
{
    [Option(Description = "The URL of the Dataverse environment")]
    [EnvVar(ConfigurationKeys.EnvironmentUrl)]
    public string Url { get; set; }

    public UserCredentials UserCredentials { get; set; }

    public ClientCredentials ClientCredentials { get; set; }
}