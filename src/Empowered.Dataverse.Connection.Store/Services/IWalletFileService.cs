using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Store.Services;

internal interface IWalletFileService
{
    void WriteWallet(ConnectionWallet wallet);
    ConnectionWallet ReadWallet();
}