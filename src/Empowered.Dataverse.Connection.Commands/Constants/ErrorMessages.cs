namespace Empowered.Dataverse.Connection.Commands.Constants;

public static class ErrorMessages
{
    public static string ConnectionTestFailed(string connectionName, string errorMessage) =>
        $"Connection test for connection {connectionName} failed with the following error message: {errorMessage}";
}