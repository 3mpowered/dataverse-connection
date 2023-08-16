using Empowered.SpectreConsole.Extensions;

namespace Empowered.Dataverse.Connection.Commands.Constants;

public static class Messages
{
    public static class Info
    {
        public const string Purging = "Purging all connections ...";
        public static string Deleting(string connectionName) => $"Removing connection {connectionName.Italic()} ...";
        public static string Using(string connectionName) => $"Using connection {connectionName.Italic()} ...";
        public static string Upserting(string connectionName) => $"Upserting connection {connectionName.Italic()} ...";
        public static string Testing(string connectionName) => $"Testing connection {connectionName.Italic()} ...";
    }

    public static class Success
    {
        public const string Purged = "Connections were successfully purged";
        public static string Deleted(string connectionName) => $"Connection {connectionName.Italic()} was successfully deleted";
        public static string Used(string connectionName) => $"Connection {connectionName.Italic()} is successfully used";

        public static string Tested(string environmentUrl, string userName) =>
            $"Successfully connected to {environmentUrl.Link()} as user {userName.Italic()}";

        public static string Upserted(string connectionName) => $"Connection {connectionName.Italic()} was successfully upserted";
    }
}