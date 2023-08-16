using System.ComponentModel.DataAnnotations;
using CommandDotNet;
using Empowered.Dataverse.Connection.Commands.Arguments;
using Empowered.Dataverse.Connection.Commands.Constants;
using Empowered.Dataverse.Connection.Store.Model;
using FluentAssertions;
using Xunit;

namespace Empowered.Dataverse.Connection.Commands.Tests.Arguments;

public class UpsertConnectionArgumentsTests
{
    private static readonly Uri ConnectionUrl = new("https://test.crm5.dynamics.com");

    private const string ConnectionName = "connection";

    // TODO: Check test
    [Fact(Skip = "This test produces a stack overflow")]
    public void ShouldValidateErrorOnDeviceCodeAndInteractiveOption()
    {
        var arguments = new UpsertConnectionArguments
        {
            ConnectionArguments = new ConnectionArguments
            {
                Url = ConnectionUrl,
                Interactive = true,
                DeviceCode = true
            },
            ConnectionNameArguments = new ConnectionNameArguments
            {
                Name = ConnectionName
            }
        };

        var validationContext = new ValidationContext(arguments);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(arguments, validationContext, validationResults);

        validationResults.Should().HaveCount(1);
        var validationResult = validationResults.Single();
        validationResult.ErrorMessage.Should().Be(ValidationErrors.DeviceCodeAndInteractive);
        validationResult.MemberNames.Should().ContainInOrder(
            nameof(ConnectionArguments.Interactive),
            nameof(ConnectionArguments.DeviceCode)
        );
    }

    [Fact]
    public void ShouldValidateErrorOnUnknownConnection()
    {
        var arguments = new UpsertConnectionArguments
        {
            ConnectionArguments = new ConnectionArguments
            {
                Url = ConnectionUrl
            },
            ConnectionNameArguments = new ConnectionNameArguments
            {
                Name = ConnectionName
            }
        };

        var validationResults = arguments.Validate(new ValidationContext(arguments));

        validationResults.Should().Contain(result => result.ErrorMessage == ValidationErrors.UnknownConnectionType);
    }

    [Fact]
    public void ShouldCreateClientSecretConnectionFromArguments()
    {
        var arguments = new UpsertConnectionArguments
        {
            ConnectionArguments = new ConnectionArguments
            {
                Url = ConnectionUrl,
                ClientSecret = new Password("12345"),
                TenantId = Guid.NewGuid().ToString(),
                ApplicationId = Guid.NewGuid().ToString()
            },
            ConnectionNameArguments = new ConnectionNameArguments
            {
                Name = ConnectionName
            }
        };

        var connection = arguments.ToConnection();

        connection.Should().BeOfType<ClientSecretConnection>();
        var clientSecretConnection = connection.As<ClientSecretConnection>();
        clientSecretConnection.Name.Should().Be(arguments.ConnectionNameArguments.Name);
        clientSecretConnection.EnvironmentUrl.Should().Be(arguments.ConnectionArguments.Url);
        clientSecretConnection.ClientSecret.Should().Be(arguments.ConnectionArguments.ClientSecret.GetPassword());
        clientSecretConnection.TenantId.Should().Be(arguments.ConnectionArguments.TenantId);
        clientSecretConnection.ApplicationId.Should().Be(arguments.ConnectionArguments.ApplicationId);
    }
    
    [Fact]
    public void ShouldCreateClientCertificateConnectionFromArguments()
    {
        var arguments = new UpsertConnectionArguments
        {
            ConnectionArguments = new ConnectionArguments
            {
                Url = ConnectionUrl,
                CertificatePassword = new Password("12345"),
                TenantId = Guid.NewGuid().ToString(),
                ApplicationId = Guid.NewGuid().ToString(),
                CertificateFilePath = new FileInfo("C:\\Temp")
            },
            ConnectionNameArguments = new ConnectionNameArguments
            {
                Name = ConnectionName
            }
        };

        var connection = arguments.ToConnection();

        connection.Should().BeOfType<ClientCertificateConnection>();
        var clientCertificateConnection = connection.As<ClientCertificateConnection>();
        clientCertificateConnection.Name.Should().Be(arguments.ConnectionNameArguments.Name);
        clientCertificateConnection.EnvironmentUrl.Should().Be(arguments.ConnectionArguments.Url);
        clientCertificateConnection.Password.Should().Be(arguments.ConnectionArguments.CertificatePassword.GetPassword());
        clientCertificateConnection.TenantId.Should().Be(arguments.ConnectionArguments.TenantId);
        clientCertificateConnection.ApplicationId.Should().Be(arguments.ConnectionArguments.ApplicationId);
        clientCertificateConnection.FilePath.Should().Be(arguments.ConnectionArguments.CertificateFilePath.FullName);
    }
    
    [Fact]
    public void ShouldCreateUserPasswordConnectionFromArguments()
    {
        var arguments = new UpsertConnectionArguments
        {
            ConnectionArguments = new ConnectionArguments
            {
                Url = ConnectionUrl,
                Password = new Password("12345"),
                TenantId = Guid.NewGuid().ToString(),
                Username = "a@b.de"
            },
            ConnectionNameArguments = new ConnectionNameArguments
            {
                Name = ConnectionName
            }
        };

        var connection = arguments.ToConnection();

        connection.Should().BeOfType<UserPasswordConnection>();
        var userPasswordConnection = connection.As<UserPasswordConnection>();
        userPasswordConnection.Name.Should().Be(arguments.ConnectionNameArguments.Name);
        userPasswordConnection.EnvironmentUrl.Should().Be(arguments.ConnectionArguments.Url);
        userPasswordConnection.Password.Should().Be(arguments.ConnectionArguments.Password.GetPassword());
        userPasswordConnection.TenantId.Should().Be(arguments.ConnectionArguments.TenantId);
        userPasswordConnection.UserName.Should().Be(arguments.ConnectionArguments.Username);
    }
    
    [Fact]
    public void ShouldCreateInteractiveConnectionFromArguments()
    {
        var arguments = new UpsertConnectionArguments
        {
            ConnectionArguments = new ConnectionArguments
            {
                Url = ConnectionUrl,
                Interactive = true
            },
            ConnectionNameArguments = new ConnectionNameArguments
            {
                Name = ConnectionName
            }
        };

        var connection = arguments.ToConnection();

        connection.Should().BeOfType<InteractiveConnection>();
        var userPasswordConnection = connection.As<InteractiveConnection>();
        userPasswordConnection.Name.Should().Be(arguments.ConnectionNameArguments.Name);
        userPasswordConnection.EnvironmentUrl.Should().Be(arguments.ConnectionArguments.Url);
    }
    
    [Fact]
    public void ShouldCreateDeviceCodeConnectionFromArguments()
    {
        var arguments = new UpsertConnectionArguments
        {
            ConnectionArguments = new ConnectionArguments
            {
                Url = ConnectionUrl,
                DeviceCode = true
            },
            ConnectionNameArguments = new ConnectionNameArguments
            {
                Name = ConnectionName
            }
        };

        var connection = arguments.ToConnection();

        connection.Should().BeOfType<DeviceCodeConnection>();
        var userPasswordConnection = connection.As<DeviceCodeConnection>();
        userPasswordConnection.Name.Should().Be(arguments.ConnectionNameArguments.Name);
        userPasswordConnection.EnvironmentUrl.Should().Be(arguments.ConnectionArguments.Url);
    }
    
    [Fact]
    public void ShouldCreateManagedIdentityConnectionFromArguments()
    {
        var arguments = new UpsertConnectionArguments
        {
            ConnectionArguments = new ConnectionArguments
            {
                Url = ConnectionUrl,
                ApplicationId = Guid.NewGuid().ToString()
            },
            ConnectionNameArguments = new ConnectionNameArguments
            {
                Name = ConnectionName
            }
        };

        var connection = arguments.ToConnection();

        connection.Should().BeOfType<ManagedIdentityConnection>();
        var managedIdentityConnection = connection.As<ManagedIdentityConnection>();
        managedIdentityConnection.Name.Should().Be(arguments.ConnectionNameArguments.Name);
        managedIdentityConnection.EnvironmentUrl.Should().Be(arguments.ConnectionArguments.Url);
        managedIdentityConnection.ClientId.Should().Be(arguments.ConnectionArguments.ApplicationId);
    }
}