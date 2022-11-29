using System.ComponentModel.DataAnnotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests
{
    public class FundTransferRequest
    {
        [Required]
        public string TransactionReference { get; set; } = string.Empty;
        [Required, RegularExpression(@"^[A-Z]0{5}[0-9]")]
        public string SourceWalletId { get; set; } = string.Empty;
        [Required, RegularExpression(@"^[A-Z]0{5}[0-9]$")]
        public string DestinationWalletId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
