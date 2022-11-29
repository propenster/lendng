using System.ComponentModel.DataAnnotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Models
{
    public class Wallet : BaseClass
    {
        [Required]
        public string WalletId { get; set; } = string.Empty; //unquely generated wallet ID...
        [Required]
        public string WalletHolderName { get; set; } = string.Empty;
        [Required]
        public string WalletHolderEmail { get; set; } = string.Empty;
        [Required]
        public string WalletCurrency { get; set; } = "NGN";
        public decimal Balance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal TotalCummulativeTransactions { get; set; } //sum total of all transactions value in decimal done by this wallet

    }
}
