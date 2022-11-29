namespace AdvansioInteractive.Service.Internal.Lendng.Helpers
{
    public class MongoDbConfig
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string WalletCollectionName { get; set; } = string.Empty;
        public string DefaultRole { get; set; } = "USER";
        public string FirstTimeWalletBonus { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = "NGN";
        public string DefaultEmail { get; set; } = string.Empty;
        public string TransactionCollectionName { get; set; } = string.Empty;
    }
}
