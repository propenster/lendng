namespace AdvansioInteractive.Service.Internal.Lendng.Helpers
{
    public static class TransferStatusCodes
    {
        public static readonly string Success = "00";
        public static readonly string DuplicateTransactionReference = "21";
        public static readonly string InsufficientBalance = "11";
        public static readonly string InvalidInactiveAccount = "88";

        public static readonly string Failed = "02";
        public static readonly string ServerException = "99";

    }
}
