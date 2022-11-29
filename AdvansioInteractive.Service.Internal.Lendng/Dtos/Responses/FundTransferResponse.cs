namespace AdvansioInteractive.Service.Internal.Lendng.Dtos.Responses
{
    public class FundTransferResponse
    {
        public string TransactionReference { get; set; } = string.Empty;
        public string TransferStatusCode { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
    }
}
