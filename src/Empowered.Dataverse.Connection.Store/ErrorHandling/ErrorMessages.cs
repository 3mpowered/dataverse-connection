using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.ErrorHandling;

internal static class ErrorMessages
{
    public const string NoActiveConnection = "Couldn't find an active connection";

    public static string ConnectionNotFound(string connectionName) => $"Couldn't find connection with name \"{connectionName}\"";

    public static string InvalidConnection(string name)
    {
        var validConnectionTypes = Enum
            .GetNames(typeof(ConnectionType))
            .Where(enumName => enumName != nameof(ConnectionType.Unknown))
            .ToList();
        return $"Connection {name} is invalid. Valid connection types are: {string.Join(Environment.NewLine, validConnectionTypes)}";
    }

    public static string ConnectionOutOfRange(Type type) => $"Connection of type is {type.Name} is out of range.";
}