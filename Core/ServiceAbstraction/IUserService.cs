using OrderMangement.Api.Shared.DataTransferObject;

namespace OrderManagement.Core.ServiceAbstraction
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(UserDto userDto);
        Task<string?> LoginAsync(string username, string password);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
    }
}
