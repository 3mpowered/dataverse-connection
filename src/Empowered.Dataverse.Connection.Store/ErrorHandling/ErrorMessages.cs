using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Store.ErrorHandling;

internal static class ErrorMessages
{
    public const string NoActiveConnection = "Couldn't find an active connection";

    public static string ConnectionNotFound(string connectionName) => $"Couldn't find connection with name \"{connectionName}\"";

    public static string InvalidConnection(string name) =>
        $"""
         Connection "{name}" is invalid. Valid connection types are {ConnectionType.ClientSecret}({nameof(ClientSecretConnection.ApplicationId)} and {nameof(ClientSecretConnection.TenantId)}), {ConnectionType.ClientCertificate}({nameof(ClientCertificateConnection.ApplicationId)}, {nameof(ClientCertificateConnection.TenantId)} and {nameof(ClientCertificateConnection.FilePath)}) or {ConnectionType.UserPassword}({nameof(UserPasswordConnection.UserName)}).
         Please validate that your connection properties match one of the existing types.
         """;
}