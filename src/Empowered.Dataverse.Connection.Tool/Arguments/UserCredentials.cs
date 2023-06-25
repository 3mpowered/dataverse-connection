using CommandDotNet;
using Empowered.Dataverse.Connection.Tool.Constants;

namespace Empowered.Dataverse.Connection.Tool.Arguments;

public class UserCredentials
{
    [Option(Description = "The username to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.Username)]
    public string Username { get; set; }

    [Option(Description = "The passwort to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.Password)]
    public Password Password { get; set; }
}