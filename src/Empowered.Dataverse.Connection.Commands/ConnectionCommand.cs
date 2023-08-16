using CommandDotNet;
using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Commands.Constants;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.SpectreConsole.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client.Utils;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Spectre.Console;
using static Empowered.Dataverse.Connection.Commands.Constants.Messages;

namespace Empowered.Dataverse.Connection.Commands;

public class ConnectionCommand
{
    private readonly IAnsiConsole _console;
    private readonly IConnectionStore _connectionStore;
    private readonly IDataverseClientFactory _dataverseClientFactory;

    public ConnectionCommand(
        IAnsiConsole console,
        IConnectionStore connectionStore,
        IDataverseClientFactory dataverseClientFactory
    )
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

        table.AddColumn(new TableColumn(ConnectionTable.Headers.Active).Centered());
        table.AddColumn(new TableColumn(ConnectionTable.Headers.ConnectionName));
        table.AddColumn(new TableColumn(ConnectionTable.Headers.ConnectionType));
        table.AddColumn(new TableColumn(ConnectionTable.Headers.EnvironmentUrl));

        var currentConnection = wallet.Current;

        foreach (var connection in wallet.Connections)
        {
            var isActiveConnection = connection.Name == currentConnection?.Name
                ? ConnectionTable.Values.ActiveFlag
                : string.Empty;

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
        _console.Info(Info.Purging);
        _connectionStore.Purge();
        _console.Success(Success.Purged);
        return await List();
    }

    public async Task<int> Delete(
        [Option(Description = "The name of the connection to delete")]
        string name
    )
    {
        _console.Info(Info.Deleting(name));
        _connectionStore.Delete(name);
        _console.Success(Success.Deleted(name));
        return await List();
    }

    public async Task<int> Use(
        [Option(Description = "The name of the connection to use")]
        string name
    )
    {
        _console.Info(Info.Using(name));
        _connectionStore.Use(name);
        _console.Success(Success.Used(name));
        return await List();
    }

    public async Task<int> Upsert(ConnectionArguments arguments)
    {
        var connectionName = arguments.Name;
        _console
            .Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Pong)
            .SpinnerStyle(Style.Parse("green"))
            .Start($"Upserting connection {connectionName.Italic()} ...", statustContext =>
            {
                statustContext.Status("Upsert Connection");
                _console.Info(Info.Upserting(connectionName));
                var connection = arguments.Clone();
                _connectionStore.Upsert(connection, true);

                if (arguments.TestConnection)
                {
                    statustContext.Status("Test Connection");
                    _console.Info(Info.Testing(connectionName));
                    var userName = WhoAmI(connectionName);
                    var environmentUrl = arguments.EnvironmentUrl.ToString();
                    _console.Success(Success.Tested(environmentUrl, userName));
                }

                _console.Success(Success.Upserted(connectionName));
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
            throw new DataverseConnectionException(ErrorMessages.ConnectionTestFailed(connectionName, exception.Message), exception);
        }
    }
}