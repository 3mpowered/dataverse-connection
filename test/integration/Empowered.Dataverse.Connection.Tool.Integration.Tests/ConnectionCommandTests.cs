using System.Runtime.CompilerServices;
using CommandDotNet;
using FluentAssertions;
using Spectre.Console;
using Xunit;
using Xunit.Abstractions;

namespace Empowered.Dataverse.Connection.Tool.Integration.Tests;

public class ConnectionCommandTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ConnectionCommandTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        AnsiConsole.Record();
    }
    [Fact(Skip = "not working at the moment")]
    public void ShouldListExistingConnections()
    {
        var arguments = new[] { "list" };
        var exitCode = Program.Main(arguments);
        exitCode.Should().Be(ExitCodes.Success.GetAwaiter().GetResult());
    }


    public void Dispose()
    {
        _testOutputHelper.WriteLine(AnsiConsole.ExportText());
    }
}