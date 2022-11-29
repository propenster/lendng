using System.ComponentModel.DataAnnotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Models
{
    public class WalletTransaction : BaseClass
    {
        public string TransactionReference { get; set; } = Guid.NewGuid().ToString();
        public string TransactionType { get; set; } = string.Empty;
        [Required, RegularExpression(@"^[A-Z]0{5}[0-9]")]
        public string SourceWalletId { get; set; } = string.Empty;
        [Required, RegularExpression(@"^[A-Z]0{5}[0-9]$")]
        public string DestinationWalletId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
    }
}
