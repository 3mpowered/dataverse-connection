using Empowered.Dataverse.Connection.Store.Contract;

namespace Empowered.Dataverse.Connection.Store.ErrorHandling;

public static class ErrorMessages
{
    public static string ConnectionNotFound(string connectionName) => $"Couldn't find connection with name \"{connectionName}\"";

    public static string InvalidConnection(IConnection connection) =>
        $"""
         Connection "{connection.Name}" is invalid.
         Valid connection types are {ConnectionType.ClientSecret}({nameof(IConnection.ApplicationId)} and {nameof(IConnection.TenantId)}),
         {ConnectionType.Certificate}({nameof(IConnection.ApplicationId)}, {nameof(IConnection.TenantId)} and {nameof(IConnection.CertificateFilePath)}
         or {ConnectionType.UserPassword}({nameof(IConnection.UserName)}).
         Please validate that your connection properties match one of the existing types.
         """;
}