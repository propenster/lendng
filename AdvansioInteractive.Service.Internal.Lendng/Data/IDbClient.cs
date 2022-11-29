using AdvansioInteractive.Service.Internal.Lendng.Models;
using MongoDB.Driver;

namespace AdvansioInteractive.Service.Internal.Lendng.Data
{
    public interface IDbClient
    {
        IMongoCollection<Wallet> GetWalletCollection();
        IMongoCollection<WalletTransaction> GetTransactionCollection();
    }
}
