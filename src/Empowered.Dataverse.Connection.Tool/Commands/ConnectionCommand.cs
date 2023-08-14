using CommandDotNet;
using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Tool.Arguments;
using Empowered.SpectreConsole.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client.Utils;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Spectre.Console;

namespace Empowered.Dataverse.Connection.Tool.Commands;

public class ConnectionCommand
{
    private readonly IAnsiConsole _console;
    private readonly IConnectionStore _connectionStore;
    private readonly IDataverseClientFactory _dataverseClientFactory;

    public ConnectionCommand(IAnsiConsole console, IConnectionStore connectionStore, IDataverseClientFactory dataverseClientFactory)
    {
        _console = console;
        _connectionStore = connectionStore;
        _dataverseClientFactory = dataverseClientFactory;
    }

    public async Task<int> List()
    {
        var wallet = _connectionStore.List();

        var table = new Table
        {
            Border = TableBorder.None,
        };

        table.AddColumn(new TableColumn("Active").Centered());
        table.AddColumn(new TableColumn("Connection Name"));
        table.AddColumn(new TableColumn("Connection Type"));
        table.AddColumn(new TableColumn("Environment URL"));

        var currentConnection = wallet.Current;

        foreach (var connection in wallet.Connections)
        {
            var isActiveConnection = connection.Name == currentConnection?.Name ? "x" : string.Empty;
            table.AddRow(
                isActiveConnection,
                connection.Name,
                connection.Type.ToString(),
                connection.EnvironmentUrl.ToString().Link()
            );
        }

        _console.Write(table);

        return await ExitCodes.Success;
    }

    public async Task<int> Purge()
    {
        _console.Info("Purging all connections ...");
        _connectionStore.Purge();
        _console.Success("Connections were successfully purged");
        return await List();
    }

    public async Task<int> Remove(ConnectionNameArguments connectionName)
    {
        _console.Info($"Removing connection {connectionName.Name.Italic()} ...");
        _connectionStore.Delete(connectionName.Name);
        _console.Success($"Connection {connectionName.Name.Italic()} was successfully deleted");
        return await List();
    }

    public async Task<int> Use(ConnectionNameArguments connectionName)
    {
        _console.Info($"Using connection {connectionName.Name.Italic()} ...");
        _connectionStore.Use(connectionName.Name);
        _console.Success($"Connection {connectionName.Name.Italic()} is successfully used");
        return await List();
    }

    public async Task<int> Upsert(UpsertConnectionArguments arguments)
    {
        var connectionName = arguments.ConnectionNameArguments.Name;
        _console
            .Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Pong)
            .SpinnerStyle(Style.Parse("green"))
            
            .Start($"Upserting connection {connectionName.Italic()} ...", statustContext =>
            {
                statustContext.Status("Upsert Connection");
                _console.Info($"Upserting connection {connectionName.Italic()} ...");
                var connection = arguments.ToConnection();
                _connectionStore.Upsert(connection, true);

                if (arguments.TestConnection)
                {
                    statustContext.Status("Test Connection");
                    _console.Info($"Testing connection {connectionName.Italic()} ...");
                    var userName = WhoAmI(connectionName);
                    _console.Success($"Successfully connected to {arguments.ConnectionArguments.Url.ToString().Link()} as user {userName.Italic()}");
                }

                _console.Success($"Connection {connectionName.Italic()} was successfully upserted");
            });
        return await List();
    }

    private string WhoAmI(string connectionName)
    {
        try
        {
            var organizationService = _dataverseClientFactory.Get<IOrganizationService>(connectionName);
            var whoAmIResponse = (WhoAmIResponse)organizationService.Execute(new WhoAmIRequest());
            var user = organizationService.Retrieve("systemuser", whoAmIResponse.UserId, new ColumnSet("fullname"));
            return user.GetAttributeValue<string>("fullname");
        }
        catch (Exception exception)
        {
            throw new DataverseConnectionException(
                $"Connection test for connection {connectionName} failed with the following error message: {exception.Message}", exception);
        }
    }
}