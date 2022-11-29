using AdvansioInteractive.Service.Internal.Lendng.Dtos.Requests;
using AdvansioInteractive.Service.Internal.Lendng.Dtos.Responses;
using AdvansioInteractive.Service.Internal.Lendng.Helpers;

namespace AdvansioInteractive.Service.Internal.Lendng.Interfaces
{
    public interface IAccountService
    {
        Task<GenericResponse<RegisterResponse>> Register(RegisterDto request);
        Task<GenericResponse<LoginResponse>> Login(LoginDto request);
    }
}
