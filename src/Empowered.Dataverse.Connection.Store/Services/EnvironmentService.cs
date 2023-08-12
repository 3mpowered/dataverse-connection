using Empowered.Dataverse.Connection.Store.Constants;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Connection.Store.Services;

internal class EnvironmentService : IEnvironmentService
{
    private readonly ILogger<EnvironmentService> _logger;
    private readonly string _localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _connectionFileName;

    public EnvironmentService(ILogger<EnvironmentService> logger)
    {
        _logger = logger;
        _connectionFileName = Application.ConnectionFile;
    }
    
    public EnvironmentService(ILogger<EnvironmentService> logger, string localAppDataPath)
    {
        _logger = logger;
        _localAppDataPath = localAppDataPath;
        _connectionFileName = Application.ConnectionFile;
    }

    public FileInfo GetConnectionFilePath()
    {
        var applicationDirectory = new DirectoryInfo(Path.Combine(_localAppDataPath, Application.Name));
        _logger.LogTrace("Getting app data path {Path} with application directory {Directory} from environment", _localAppDataPath,
            applicationDirectory.Name);

        if (!applicationDirectory.Exists)
        {
            _logger.LogTrace("Application directory {Directory} doesn't exist -> create directory", applicationDirectory.FullName);
            applicationDirectory.Create();
        }

        var connectionFilePath = Path.Combine(applicationDirectory.FullName, _connectionFileName);
        _logger.LogTrace("Returning connection file {Filename} in path {Path}", _connectionFileName, connectionFilePath);

        return new FileInfo(connectionFilePath);
    }
}