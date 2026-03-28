using HardwareStore.Application.DTOs.Auth;

namespace HardwareStore.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResultDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
