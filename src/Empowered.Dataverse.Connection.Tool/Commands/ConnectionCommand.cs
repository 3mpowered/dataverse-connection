using CommandDotNet;
using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Tool.Arguments;
using Empowered.SpectreConsole.Extensions;
using Microsoft.Crm.Sdk.Messages;
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
        table.AddColumn(new TableColumn("Connection Name").Centered());
        table.AddColumn(new TableColumn("Environment URL").Centered());

        var currentConnection = wallet.Current;

        foreach (var connection in wallet.Connections)
        {
            var isActiveConnection = connection.Name == currentConnection?.Name ? "x" : string.Empty;
            table.AddRow(isActiveConnection, connection.Name, connection.EnvironmentUrl.ToString().Link());
        }

        _console.Write(table);

        return await ExitCodes.Success;
    }

    public async Task<int> Purge()
    {
        _console.Info("Purging all connections ...");
        try
        {
            _connectionStore.Purge();
        }
        catch (Exception exception)
        {
            _console.Error(exception.Message);
            return await ExitCodes.Error;
        }

        _console.Success("Connections were successfully purged");
        return await List();
    }

    public async Task<int> Remove(ConnectionNameArguments connectionName)
    {
        _console.Info($"Removing connection {connectionName.Name.Italic()} ...");
        try
        {
            _connectionStore.Delete(connectionName.Name);
        }
        catch (Exception exception)
        {
            _console.Error(exception.Message);
            return await ExitCodes.Error;
        }

        _console.Success($"Connection {connectionName.Name.Italic()} was successfully deleted");
        return await List();
    }

    public async Task<int> Use(ConnectionNameArguments connectionName)
    {
        _console.Info($"Using connection {connectionName.Name.Italic()} ...");
        try
        {
            _connectionStore.Use(connectionName.Name);
        }
        catch (Exception exception)
        {
            _console.Error(exception.Message);
            return await ExitCodes.Error;
        }

        _console.Success($"Connection {connectionName.Name.Italic()} is successfully used");
        return await List();
    }

    public async Task<int> Upsert(UpsertConnectionArguments arguments)
    {
        var connectionName = arguments.ConnectionNameArguments.Name;
        try
        {
            _console.Info($"Upserting connection {connectionName.Italic()} ...");
            var connection = arguments.ToConnection();
            var secret = arguments.ConnectionArguments.GetSecret();

            if (secret == null)
            {
                _console.Error(
                    $"No secret provided for connection {connectionName}. Please provide either a password, client secret or certificate password.");
                return await ExitCodes.Error;
            }

            _connectionStore.Upsert(connection, secret.GetPassword(), true);
        }
        catch (Exception exception)
        {
            _console.Error(exception.Message);
            return await ExitCodes.Error;
        }

        try
        {
            var organizationService = _dataverseClientFactory.Get<IOrganizationService>(connectionName);
            var whoAmIResponse = (WhoAmIResponse)organizationService.Execute(new WhoAmIRequest());
            var user = organizationService.Retrieve("systemuser", whoAmIResponse.UserId, new ColumnSet("fullname"));
            var userName = user.GetAttributeValue<string>("fullname");
            _console.Success($"Successfully connected to {arguments.ConnectionArguments.Url.Link()} as user {userName.Italic()}");
        }
        catch (Exception exception)
        {
            _console.Error($"Connection test for connection {connectionName.Italic()} failed with error message: {exception.Message.EscapeMarkup()}");
            return await ExitCodes.Error;
        }
        
        _console.Success($"Connection {connectionName.Italic()} was successfully upserted");
        return await List();
    }
}