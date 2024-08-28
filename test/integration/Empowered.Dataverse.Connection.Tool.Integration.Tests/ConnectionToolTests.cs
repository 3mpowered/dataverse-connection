using System.Reflection;
using CommandDotNet;
using CommandDotNet.Spectre.Testing;
using CommandDotNet.TestTools.Scenarios;
using Empowered.Dataverse.CommandDotnet.Xunit.Extensions;
using Empowered.Dataverse.Connection.Client.Extensions;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Tool.Integration.Tests.Constants;
using Empowered.Dataverse.Connection.Tool.Integration.Tests.Extensions;
using FluentAssertions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.Client;
using Xunit;
using Xunit.Abstractions;

namespace Empowered.Dataverse.Connection.Tool.Integration.Tests;

public class ConnectionToolTests : IDisposable
{
    private const string ConnectionName = "connection";
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AnsiTestConsole _ansiTestConsole;
    private readonly IConfiguration _configuration;

    public ConnectionToolTests(ITestOutputHelper testOutputHelper)
    {
        Ambient.Output = testOutputHelper;
        _testOutputHelper = testOutputHelper;
        _ansiTestConsole = new AnsiTestConsole();

        _configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables("3MPWRD:TESTS")
            .AddJsonFile("local.settings.json", optional: true)
            .Build();
    }

    [Fact]
    public async Task ShouldListExistingConnections()
    {
        Program.GetAppRunner(_ansiTestConsole)
            .Verify(new Scenario
                {
                    When = { Args = "list" },
                    Then =
                    {
                        ExitCode = await ExitCodes.Success,
                    }
                }
            );
    }

    [Fact]
    public void ShouldAddInteractiveConnection()
    {
        const string connectionName = "connection";
        var environmentUrl = _configuration.GetRequiredValue<string>(ConfigurationKeys.EnvironmentUrl);
        Program.GetAppRunner(_ansiTestConsole)
            .Verify(new Scenario
            {
                When =
                {
                    ArgsArray =
                    [
                        "upsert",
                        "--name",
                        connectionName,
                        "--environment-url",
                        environmentUrl,
                        "--type",
                        ConnectionType.Interactive.ToString(),
                        "--skip-connection-test"
                    ]
                }
            });
    }

    [Fact]
    public void ShouldAddAndTestClientSecretConnection()
    {
        const string connectionName = "connection";
        var environmentUrl = _configuration.GetRequiredValue<string>(ConfigurationKeys.EnvironmentUrl);
        var tenantId = _configuration.GetRequiredValue<string>(ConfigurationKeys.TenantId);
        var applicationId = _configuration.GetRequiredValue<string>(ConfigurationKeys.ApplicationId);
        var clientSecret = _configuration.GetRequiredValue<string>(ConfigurationKeys.ClientSecret);
        Program.GetAppRunner(_ansiTestConsole)
            .Verify(new Scenario
            {
                When =
                {
                    ArgsArray =
                    [
                        "upsert",
                        "--name",
                        connectionName,
                        "--environment-url",
                        environmentUrl,
                        "--type",
                        ConnectionType.ClientSecret.ToString(),
                        "--tenant-id",
                        tenantId,
                        "--application-id",
                        applicationId,
                        "--client-secret",
                        clientSecret
                    ]
                }
            });
    }

    [Fact(Skip = "Currently missing a test user without mfa")]
    public void ShouldAddAndTestUserPasswordConnection()
    {
        const string connectionName = "connection";
        var environmentUrl = _configuration.GetRequiredValue<string>(ConfigurationKeys.EnvironmentUrl);
        var tenantId = _configuration.GetRequiredValue<string>(ConfigurationKeys.TenantId);
        var userName = _configuration.GetRequiredValue<string>(ConfigurationKeys.UserName);
        var password = _configuration.GetRequiredValue<string>(ConfigurationKeys.Password);
        Program.GetAppRunner(_ansiTestConsole)
            .Verify(new Scenario
            {
                When =
                {
                    ArgsArray =
                    [
                        "upsert",
                        "--name",
                        connectionName,
                        "--environment-url",
                        environmentUrl,
                        "--type",
                        ConnectionType.UserPassword.ToString(),
                        "--tenant-id",
                        tenantId,
                        "--user-name",
                        userName,
                        "--password",
                        password
                    ]
                }
            });
    }

    [Fact(Skip = "Certificate expired")]
    public void ShouldAddAndTestClientCertificateConnection()
    {
        var environmentUrl = _configuration.GetRequiredValue<string>(ConfigurationKeys.EnvironmentUrl);
        var tenantId = _configuration.GetRequiredValue<string>(ConfigurationKeys.TenantId);
        var applicationId = _configuration.GetRequiredValue<string>(ConfigurationKeys.ApplicationId);
        var certFilePath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName, "local", "certificate.pfx");
        var password = _configuration.GetRequiredValue<string>(ConfigurationKeys.CertificatePassword);
        Program.GetAppRunner(_ansiTestConsole)
            .Verify(new Scenario
            {
                When =
                {
                    ArgsArray = new[]
                    {
                        "upsert",
                        "--name",
                        ConnectionName,
                        "--environment-url",
                        environmentUrl,
                        "--type",
                        ConnectionType.ClientCertificate.ToString(),
                        "--tenant-id",
                        tenantId,
                        "--application-id",
                        applicationId,
                        "--certificate-file-path",
                        certFilePath,
                        "--certificate-password",
                        password
                    }
                }
            });
    }

    [Fact]
    public void ShouldInjectNamedServiceClient()
    {
        ShouldAddAndTestClientSecretConnection();

        var configuration = new ConfigurationBuilder()
            .AddDataverseConnectionSource(ConnectionName)
            .Build();
        
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddDataverseClient<ServiceClient>()
            .BuildServiceProvider();
        
        var serviceClient = serviceProvider.GetRequiredService<ServiceClient>();

        serviceClient.Should().NotBeNull();

        var whoAmIResponse =(WhoAmIResponse) serviceClient.Execute(new WhoAmIRequest());

        whoAmIResponse.Should().NotBeNull();
    }

    public void Dispose()
    {
        _testOutputHelper.WriteLine(_ansiTestConsole.Output);
    }
}