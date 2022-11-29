using AdvansioInteractive.Service.Internal.Lendng.Data;
using AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests;
using AdvansioInteractive.Service.Internal.Lendng.Dtos.Responses;
using AdvansioInteractive.Service.Internal.Lendng.Enums;
using AdvansioInteractive.Service.Internal.Lendng.Helpers;
using AdvansioInteractive.Service.Internal.Lendng.Interfaces;
using AdvansioInteractive.Service.Internal.Lendng.Models;
using MongoDB.Driver;

namespace AdvansioInteractive.Service.Internal.Lendng.Services
{
    public class CoreService : ICoreService
    {
        private readonly ILogger _logger;
        private readonly IMongoCollection<Wallet> _wallets;
        private readonly IMongoCollection<WalletTransaction> _transactions;
        private readonly FilterDefinitionBuilder<Wallet> walletFilterBuilder = Builders<Wallet>.Filter;
        private readonly FilterDefinitionBuilder<WalletTransaction> transactionFilterBuilder = Builders<WalletTransaction>.Filter;
        public CoreService(IDbClient dbClient, ILogger<CoreService> logger)
        {
            _wallets = dbClient.GetWalletCollection();
            _transactions = dbClient.GetTransactionCollection();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GenericResponse<FundTransferResponse>> FundTransfer(FundTransferRequest request)
        {
            try
            {
                //FilterDefinition <Wallet> filter = walletFilterBuilder.Eq(x => x.IsActive && x.WalletId, true );
                var sourceWallet = await _wallets.Find(x => x.Id == Guid.Parse(request.SourceWalletId) && x.IsActive).FirstOrDefaultAsync();
                var transaction = await _transactions.Find(x => x.TransactionReference == request.TransactionReference).FirstOrDefaultAsync();
                if(transaction is not null)
                {
                    return new GenericResponse<FundTransferResponse>
                    {
                        Data = new FundTransferResponse { TransferStatusCode = TransferStatusCodes.DuplicateTransactionReference },
                        Message = "Failed. Duplicate transaction detected.",
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Success = false,
                    };
                }
                if (sourceWallet is null)
                {
                    return new GenericResponse<FundTransferResponse>
                    {
                        Data = new FundTransferResponse { TransferStatusCode = TransferStatusCodes.InvalidInactiveAccount},
                        Message = "Failed. Invalid source wallet",
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Success = false,
                    };
                }
                var desinationWallet = await _wallets.Find(x => x.Id == Guid.Parse(request.SourceWalletId) && x.IsActive).FirstOrDefaultAsync();
                if (desinationWallet is null)
                {
                    return new GenericResponse<FundTransferResponse>
                    {
                        Data = new FundTransferResponse { TransferStatusCode = TransferStatusCodes.InvalidInactiveAccount },
                        Message = "Failed. Invalid destination wallet",
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Success = false,
                    };
                }

                if(sourceWallet?.Balance < request.Amount)
                {
                    return new GenericResponse<FundTransferResponse>
                    {
                        Data = new FundTransferResponse { TransferStatusCode = TransferStatusCodes.InsufficientBalance },
                        Message = "Failed. Insufficient balance.",
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Success = false,
                    };
                }

                //debit source and credit...
                //Automapper could have done this quite elegantly BUT I think I will take my chances here... do it raw manual...
                transaction = new WalletTransaction
                {
                    TransactionDate = DateTime.Now,
                    Amount = request.Amount,
                    DestinationWalletId = desinationWallet?.WalletId,
                    SourceWalletId = sourceWallet?.WalletId,
                    TransactionType = TransactionType.Transfer.ToString().ToUpper(),
                    TransactionReference = request.TransactionReference,

                };
                await _transactions.InsertOneAsync(transaction);

                sourceWallet.Balance -= transaction.Amount;
                desinationWallet.Balance += transaction.Amount;

                await _wallets.ReplaceOneAsync(x => x.Id == sourceWallet.Id, sourceWallet);
                await _wallets.ReplaceOneAsync(x => x.Id == desinationWallet.Id, desinationWallet);

                return new GenericResponse<FundTransferResponse>
                {
                    Data = new FundTransferResponse
                    {
                        TransactionReference = transaction.TransactionReference,
                        Amount = transaction.Amount.ToString(),
                        BeneficiaryName = desinationWallet?.WalletHolderName,
                        SenderName = sourceWallet?.WalletHolderName,
                        TransferStatusCode = TransferStatusCodes.Success,
                    },
                    Message = "Success. Transfer Successful",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {

                _logger.LogError($"{nameof(FundTransfer)} ERROR OCCURRED => {ex.Message } INNER EXCEPTION => {ex.InnerException} STACKTRACE => {ex.StackTrace}");
                Console.WriteLine(ex);
                return new GenericResponse<FundTransferResponse>
                {
                    Data = new FundTransferResponse { TransferStatusCode = TransferStatusCodes.ServerException },
                    Message = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Success = false,
                };
            }
        }

        public async Task<GenericResponse<decimal>> GetBalance(string walletId)
        {
            try
            {
                //FilterDefinition <Wallet> filter = walletFilterBuilder.Eq(x => x.IsActive && x.WalletId, true );
                var sourceWallet = await _wallets.Find(x => x.Id == Guid.Parse(walletId) && x.IsActive).FirstOrDefaultAsync();
                if (sourceWallet is not null)
                {
                    return new GenericResponse<decimal>
                    {
                        Data = 0,
                        Message = "Failed. Invalid wallet ID.",
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Success = false,
                    };
                }
                

                return new GenericResponse<decimal>
                {
                    Data = sourceWallet.AvailableBalance,
                    Message = "Success. Transfer Successful",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {

                _logger.LogError($"{nameof(FundTransfer)} ERROR OCCURRED => {ex.Message } INNER EXCEPTION => {ex.InnerException} STACKTRACE => {ex.StackTrace}");
                Console.WriteLine(ex);
                return new GenericResponse<decimal>
                {
                    Data = 0,
                    Message = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Success = false,
                };
            }
        }
    }
}
