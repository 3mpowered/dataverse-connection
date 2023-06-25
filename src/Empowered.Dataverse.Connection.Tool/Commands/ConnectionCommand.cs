using Empowered.Dataverse.Connection.Tool.Arguments;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Spectre.Console;

namespace Empowered.Dataverse.Connection.Tool.Commands;

public class ConnectionCommand
{
    private readonly IAnsiConsole _console;

    public ConnectionCommand(IAnsiConsole console)
    {
        _console = console;
    }

    public async Task<int> List()
    {

        return 0;
    }

    public async Task<int> Clear()
    {
        return 0;
    }

    public async Task<int> Remove(ConnectionNameArguments connectionName)
    {
        return 0;
    }

    public async Task<int> Use(ConnectionNameArguments connectionName)
    {

        return 0;
    }

    public async Task<int> Upsert(UpsertConnectionArguments arguments)
    {
        _console.MarkupLine("[bold yellow]Add new connection[/] [red]{0}[/]", arguments.ConnectionNameArguments.Name);

        var serviceClient = new ServiceClient("");
        var response = (WhoAmIResponse)await serviceClient.ExecuteAsync(new WhoAmIRequest());

        var table = new Table();
        table.AddColumn(new TableColumn("Parameter").Centered());
        table.AddColumn(new TableColumn("Value").Centered());
        table.AddRow(nameof(response.OrganizationId), response.OrganizationId.ToString("D"));
        table.AddRow(nameof(response.BusinessUnitId), response.BusinessUnitId.ToString("D"));
        table.AddRow(nameof(response.UserId), response.UserId.ToString("D"));
        _console.Write(table);

        return 0;
    }
}