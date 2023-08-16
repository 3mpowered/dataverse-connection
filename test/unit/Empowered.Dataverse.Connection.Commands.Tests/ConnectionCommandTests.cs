using System.ServiceModel;
using System.Text.RegularExpressions;
using CommandDotNet;
using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Commands.Constants;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Model;
using Empowered.SpectreConsole.Extensions;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client.Utils;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Spectre.Console;
using Spectre.Console.Testing;
using Xunit;

namespace Empowered.Dataverse.Connection.Commands.Tests;

public partial class ConnectionCommandTests
{
    private readonly ConnectionCommand _connectionCommand;
    private readonly IDataverseClientFactory _dataverseClientFactory;
    private readonly IConnectionStore _connectionStore;

    private static readonly Uri EnvironmentUrl = new("https://test.crm16.dynamics.com");
    private readonly Recorder _consoleRecorder;

    [GeneratedRegex("\\s{2,}")]
    private static partial Regex MultipleWhitespaceCharacters();

    public ConnectionCommandTests()
    {
        _dataverseClientFactory = A.Fake<IDataverseClientFactory>();
        _connectionStore = A.Fake<IConnectionStore>();

        var console = new TestConsole();
        _consoleRecorder = new Recorder(console);
        _connectionCommand = new ConnectionCommand(_consoleRecorder, _connectionStore, _dataverseClientFactory);
    }

    [Fact]
    public async Task ShouldListExistingConnectionsInTable()
    {
        var interactiveConnection = DataverseConnection.InteractiveConnection("interactive", EnvironmentUrl);
        var deviceCodeConnection = DataverseConnection.DeviceCodeConnection("device-code", EnvironmentUrl);
        var connectionWallet = A.Fake<IConnectionWallet>();
        var connections = new HashSet<DataverseConnection>
        {
            interactiveConnection,
            deviceCodeConnection
        };

        A.CallTo(() => connectionWallet.Connections).ReturnsLazily(() => connections);
        A.CallTo(() => connectionWallet.Current).ReturnsLazily(() => deviceCodeConnection);
        A.CallTo(() => _connectionStore.List()).ReturnsLazily(() => connectionWallet);

        var exitCode = await _connectionCommand.List();
        var consoleOutput = _consoleRecorder.ExportText();

        exitCode.Should().Be(await ExitCodes.Success);

        consoleOutput.Should().NotBeNull();
        var rows = consoleOutput.Split("\n", StringSplitOptions.TrimEntries);

        rows.Should().HaveCount(3);

        var headers = rows.First();

        headers.Should().ContainAll(
            ConnectionTable.Headers.Active,
            ConnectionTable.Headers.ConnectionName,
            ConnectionTable.Headers.ConnectionType,
            ConnectionTable.Headers.EnvironmentUrl
        );

        var firstRow = MultipleWhitespaceCharacters().Split(rows.Skip(1).First());

        firstRow.Should().HaveCount(3); // Active flag is whitespace --> only 3 entries.
        firstRow[0].Should().Be(interactiveConnection.Name);
        firstRow[1].Should().Be(interactiveConnection.Type.ToString());
        firstRow[2].Should().Be(interactiveConnection.EnvironmentUrl.ToString());

        var secondRow = MultipleWhitespaceCharacters().Split(rows.Last());
        secondRow.Should().HaveCount(4);
        secondRow[0].Should().Be(ConnectionTable.Values.ActiveFlag);
        secondRow[1].Should().Be(deviceCodeConnection.Name);
        secondRow[2].Should().Be(deviceCodeConnection.Type.ToString());
        secondRow[3].Should().Be(deviceCodeConnection.EnvironmentUrl.ToString());
    }

    [Fact]
    public async Task ShouldSuccessfullyPurgeConnections()
    {
        A.CallTo(() => _connectionStore.Purge())
            .DoesNothing();

        var exitCode = await _connectionCommand.Purge();
        exitCode.Should().Be(await ExitCodes.Success);
        A.CallTo(() => _connectionStore.Purge())
            .MustHaveHappenedOnceExactly();
        var consoleOutput = _consoleRecorder.ExportText();
        consoleOutput.Should().Contain(Messages.Info.Purging);
        consoleOutput.Should().Contain(Messages.Success.Purged);
    }

    [Fact]
    public async Task ShouldSuccessfullyRemoveExistingConnection()
    {
        A.CallTo(() => _connectionStore.Delete(A<string>._))
            .DoesNothing();
        const string connectionName = "connection";

        var exitCode = await _connectionCommand.Delete(connectionName);

        exitCode.Should().Be(await ExitCodes.Success);
        A.CallTo(() => _connectionStore.Delete(A<string>._))
            .MustHaveHappenedOnceExactly();
        var consoleOutput = _consoleRecorder.ExportText();
        consoleOutput.Should().Contain(Messages.Info.Deleting(connectionName).ApplyMarkup());
        consoleOutput.Should().Contain(Messages.Success.Deleted(connectionName).ApplyMarkup());
    }

    [Fact]
    public async Task ShouldSuccessfullyUseConnection()
    {
        A.CallTo(() => _connectionStore.Use(A<string>._))
            .DoesNothing();
        const string connectionName = "connection";

        var exitCode = await _connectionCommand.Use(connectionName);

        exitCode.Should().Be(await ExitCodes.Success);
        A.CallTo(() => _connectionStore.Use(A<string>._))
            .MustHaveHappenedOnceExactly();
        var consoleOutput = _consoleRecorder.ExportText();
        consoleOutput.Should().Contain(Messages.Info.Using(connectionName).ApplyMarkup());
        consoleOutput.Should().Contain(Messages.Success.Used(connectionName).ApplyMarkup());
    }

    [Fact]
    public async Task ShouldSuccessfullyUpsertAndTestConnection()
    {
        A.CallTo(() => _connectionStore.Upsert(A<IDataverseConnection>._, A<bool>._))
            .DoesNothing();

        var organizationService = A.Fake<IOrganizationService>();
        A.CallTo(() => _dataverseClientFactory.Get<IOrganizationService>(A<string>._))
            .Returns(organizationService);

        A.CallTo(() => organizationService.Execute(A<WhoAmIRequest>._))
            .Returns(new WhoAmIResponse
            {
                [nameof(WhoAmIResponse.UserId)] = Guid.NewGuid()
            });
        const string username = "a user";
        A.CallTo(() => organizationService.Retrieve("systemuser", A<Guid>._, A<ColumnSet>._))
            .Returns(new Entity("systemuser")
            {
                ["fullname"] = username
            });

        var arguments = new ConnectionArguments
        {
            Name = "connection",
            EnvironmentUrl = EnvironmentUrl,
            Type = ConnectionType.Interactive,
            SkipConnectionTest = false
        };


        var exitCode = await _connectionCommand.Upsert(arguments);

        exitCode.Should().Be(await ExitCodes.Success);
        A.CallTo(() => _connectionStore.Upsert(A<IDataverseConnection>._, A<bool>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _dataverseClientFactory.Get<IOrganizationService>(A<string>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationService.Execute(A<WhoAmIRequest>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationService.Retrieve("systemuser", A<Guid>._, A<ColumnSet>._))
            .MustHaveHappenedOnceExactly();
        var consoleOutput = _consoleRecorder.ExportText();
        consoleOutput.Should().Contain(Messages.Info.Upserting(arguments.Name).ApplyMarkup());
        consoleOutput.Should().Contain(Messages.Info.Testing(arguments.Name).ApplyMarkup());
        consoleOutput.Should().Contain(Messages.Success.Tested(EnvironmentUrl.ToString(), username).ApplyMarkup());
        consoleOutput.Should().Contain(Messages.Success.Upserted(arguments.Name).ApplyMarkup());
    }

    [Fact]
    public async Task SHouldThrowOnConnectionTest()
    {
        A.CallTo(() => _connectionStore.Upsert(A<IDataverseConnection>._, A<bool>._))
            .DoesNothing();

        var organizationService = A.Fake<IOrganizationService>();
        A.CallTo(() => _dataverseClientFactory.Get<IOrganizationService>(A<string>._))
            .Returns(organizationService);
        A.CallTo(() => organizationService.Execute(A<WhoAmIRequest>._))
            .Throws<FaultException>();

        var arguments = new ConnectionArguments
        {
            Name = "connection",
            EnvironmentUrl = EnvironmentUrl,
            Type = ConnectionType.Interactive,
            SkipConnectionTest = false
        };

        var action = () => _connectionCommand.Upsert(arguments);

        var assertion = await action
            .Should()
            .ThrowExactlyAsync<DataverseConnectionException>();
        assertion
            .Where(
                exception => exception.Message.StartsWith(ErrorMessages.ConnectionTestFailed(arguments.Name, string.Empty)))
            .WithInnerExceptionExactly<FaultException>();

        A.CallTo(() => _connectionStore.Upsert(A<IDataverseConnection>._, A<bool>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _dataverseClientFactory.Get<IOrganizationService>(A<string>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationService.Execute(A<WhoAmIRequest>._))
            .MustHaveHappenedOnceExactly();
        var consoleOutput = _consoleRecorder.ExportText();

        consoleOutput.Should().Contain(Messages.Info.Upserting(arguments.Name).ApplyMarkup());
        consoleOutput.Should().Contain(Messages.Info.Testing(arguments.Name).ApplyMarkup());
    }
}