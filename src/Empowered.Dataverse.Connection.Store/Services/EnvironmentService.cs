using Empowered.Dataverse.Connection.Store.Constants;

namespace Empowered.Dataverse.Connection.Store.Services;

internal class EnvironmentService : IEnvironmentService
{
    private readonly string _connectionFileName;

    public EnvironmentService(string connectionFileName = Application.ConnectionFile)
    {
        _connectionFileName = connectionFileName;
    }

    public FileInfo GetConnectionFilePath()
    {
        var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var applicationDirectory = new DirectoryInfo(Path.Combine(localAppDataPath, Application.Name));

        if (!applicationDirectory.Exists)
        {
            applicationDirectory.Create();
        }

        var connectionFilePath = Path.Combine(applicationDirectory.FullName, _connectionFileName);

        return new FileInfo(connectionFilePath);
    }
}