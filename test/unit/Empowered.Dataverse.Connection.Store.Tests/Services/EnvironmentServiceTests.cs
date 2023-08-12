using System.Reflection;
using Empowered.Dataverse.Connection.Store.Constants;
using Microsoft.Extensions.Logging.Abstractions;

namespace Empowered.Dataverse.Connection.Store.Services;

public class EnvironmentServiceTests
{
    private readonly string _localAssemblyDir = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;

    [Fact]
    public void ShouldGetConnectionFilePathInLocalAppDirectory()
    {
        var environmentService = new EnvironmentService(NullLogger<EnvironmentService>.Instance, _localAssemblyDir);

        var connectionFilePath = environmentService.GetConnectionFilePath();

        connectionFilePath.Name.Should().Be(Application.ConnectionFile);
        connectionFilePath.DirectoryName.Should().StartWith(_localAssemblyDir);
        connectionFilePath.DirectoryName.Should().EndWith(Application.Name);
    }

    [Fact]
    public void ShouldCreateApplicationDirectory()
    {
        var environmentService = new EnvironmentService(NullLogger<EnvironmentService>.Instance, _localAssemblyDir);
        var connectionFilePath = environmentService.GetConnectionFilePath();
        connectionFilePath.Directory!.Delete(true);

        var newConnectionFilePath = environmentService.GetConnectionFilePath();
        newConnectionFilePath.Directory.Should().NotBeNull();
        newConnectionFilePath.Directory!.Exists.Should().BeTrue();
    }
}