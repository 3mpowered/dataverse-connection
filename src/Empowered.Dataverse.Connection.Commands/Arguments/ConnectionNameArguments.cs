using CommandDotNet;
using Empowered.Dataverse.Connection.Commands.Constants;

namespace Empowered.Dataverse.Connection.Commands.Arguments;

public class ConnectionNameArguments : IArgumentModel
{
    [Option(Description = "The name to identify the connection")]
    [EnvVar(ConfigurationKeys.ConnectionName)]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Name { get; init; }
}