using System.ComponentModel.DataAnnotations;

namespace AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests
{
    public class BalanceEnquiryRequest
    {
        [Required, RegularExpression(@"^[A-Z]0{5}[0-9]")]
        public string WalletId { get; set; } = string.Empty;
    }
}
