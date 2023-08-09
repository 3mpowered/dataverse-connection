using Empowered.Dataverse.Connection.Store.Constants;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Connection.Store.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly ILogger<EnvironmentService> _logger;
    private readonly string _connectionFileName;

    public EnvironmentService(ILogger<EnvironmentService> logger)
    {
        _logger = logger;
        _connectionFileName = Application.ConnectionFile;
    }

    public FileInfo GetConnectionFilePath()
    {
        var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var applicationDirectory = new DirectoryInfo(Path.Combine(localAppDataPath, Application.Name));
        _logger.LogTrace("Getting app data path {Path} with application directory {Directory} from environment", localAppDataPath,
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