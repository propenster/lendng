using AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests;
using AdvansioInteractive.Service.Internal.Lendng.Dtos.Responses;
using AdvansioInteractive.Service.Internal.Lendng.Helpers;

namespace AdvansioInteractive.Service.Internal.Lendng.Interfaces
{
    public interface ICoreService
    {
        Task<GenericResponse<decimal>> GetBalance(string walletId);
        Task<GenericResponse<FundTransferResponse>> FundTransfer(FundTransferRequest request);
    }
}
