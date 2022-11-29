using AdvansioInteractive.Service.Internal.Lendng.Helpers;
using AdvansioInteractive.Service.Internal.Lendng.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AdvansioInteractive.Service.Internal.Lendng.Data
{
    public class DbClient : IDbClient
    {
        private readonly MongoDbConfig _mongoConfig;
        private readonly IMongoCollection<Wallet> _wallets;
        private readonly IMongoCollection<WalletTransaction> _transactions;

        public DbClient(IOptions<MongoDbConfig> mongoConfig)
        {
            _mongoConfig = mongoConfig.Value ?? throw new ArgumentNullException(nameof(mongoConfig));
            var settings = MongoClientSettings.FromConnectionString(_mongoConfig.ConnectionString);
            settings.SslSettings = new SslSettings()
            {
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
            };
            var client = new MongoClient(settings);
            var database = client.GetDatabase(_mongoConfig.DatabaseName);
            _wallets = database.GetCollection<Wallet>(_mongoConfig.WalletCollectionName);
            _transactions = database.GetCollection<WalletTransaction>(_mongoConfig.TransactionCollectionName);
        }

        public IMongoCollection<WalletTransaction> GetTransactionCollection() => _transactions;
        
        public IMongoCollection<Wallet> GetWalletCollection() => _wallets;
        
    }
}
