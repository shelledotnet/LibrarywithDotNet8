using Books.domain.Models;
using Books.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Service
{
    public interface IUserRepository
    {
        Task<ServiceResponse<GenTokens>> Authenticate(LoginRequestDto loginRequestDto);

        Task<ServiceResponse<LoginResponseDto>> Register(RegisterRequestDto registerRequestDto);

        Task<ServiceResponse<GenTokens>> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto);

        Task<bool> UserAlreadyExists(string username, string email);
    }
}
