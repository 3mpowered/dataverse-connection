using CommandDotNet;
using Empowered.Dataverse.Connection.Tool.Constants;

namespace Empowered.Dataverse.Connection.Tool.Arguments;

public class ConnectionNameArguments : IArgumentModel
{
    [Option(Description = "The name to identify the connection")]
    [EnvVar(ConfigurationKeys.ConnectionName)]
    public string Name { get; set; }
}