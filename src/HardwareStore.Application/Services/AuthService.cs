using HardwareStore.Application.Abstractions;
using HardwareStore.Application.DTOs.Auth;
using HardwareStore.Application.Interfaces;
using HardwareStore.Domain.Entities;
using HardwareStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HardwareStore.Application.Services;

public sealed class AuthService(IAppDbContext dbContext, IPasswordHasher passwordHasher, IAuditService auditService) : IAuthService
{
    public async Task<LoginResultDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return new LoginResultDto { ErrorMessage = "Username and password are required." };
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(
            x => x.Username == request.Username.Trim(),
            cancellationToken);

        var succeeded = user is not null && user.IsActive && passwordHasher.Verify(user.PasswordHash, request.Password);

        await auditService.WriteAsync(
            AuditActionType.LoginAttempt,
            nameof(User),
            user?.Id.ToString(),
            user?.Id,
            succeeded ? "Successful login." : $"Failed login for username '{request.Username}'.",
            cancellationToken: cancellationToken);

        if (!succeeded || user is null)
        {
            return new LoginResultDto { ErrorMessage = "Invalid username or password." };
        }

        user.LastLoginUtc = DateTime.UtcNow;
        dbContext.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LoginResultDto
        {
            Succeeded = true,
            UserId = user.Id,
            DisplayName = user.DisplayName,
            Username = user.Username,
            Role = user.Role
        };
    }
}
