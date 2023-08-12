using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.ErrorHandling;

internal static class ErrorMessages
{
    public const string NoActiveConnection = "Couldn't find an active connection";
    public static string MissingSecret(string name) => $"A connection secret for connection \"{name}\" doesn't exist";

    public static string ConnectionNotFound(string connectionName) => $"Couldn't find connection with name \"{connectionName}\"";

    public static string InvalidConnection(string name) =>
        $"""
         Connection "{name}" is invalid. Valid connection types are {ConnectionType.ClientSecret}({nameof(IConnection.ApplicationId)} and {nameof(IConnection.TenantId)}), {ConnectionType.Certificate}({nameof(IConnection.ApplicationId)}, {nameof(IConnection.TenantId)} and {nameof(IConnection.CertificateFilePath)}) or {ConnectionType.UserPassword}({nameof(IConnection.UserName)}).
         Please validate that your connection properties match one of the existing types.
         """;
}