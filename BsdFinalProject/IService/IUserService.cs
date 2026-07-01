using BsdFinalProject.DTOs;

namespace BsdFinalProject.IService
{
    public interface IUserService
    {
        abstract bool VerifyPassword(string hashedPassword, string password);
        Task<(bool Success, string? Token, string? Error)> LoginAsync(LoginDto dto);
        Task<(bool Success, string? Token, string? Error)> UserRegister(CreateUserDto dto);
    }
}