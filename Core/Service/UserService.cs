using AutoMapper;
using OrderManagement.Core.DomainLayer.Contracts;
using OrderManagement.Core.DomainLayer.Entities;
using OrderManagement.Core.ServiceAbstraction;
using OrderMangement.Api.Shared.DataTransferObject;
using BCrypt.Net;

namespace OrderManagement.Core.Service
{
    public class UserService(IUnitOfWork _unitOfWork, IMapper _mapper, IJwtTokenService _jwtTokenService) : IUserService
    {

        public async Task<UserDto> RegisterUserAsync(UserDto userDto)
        {
            var existingUser = await _unitOfWork.Users.UsernameExistsAsync(userDto.Username);
            if (existingUser)
                throw new InvalidOperationException("Username already exists");

            var user = _mapper.Map<User>(userDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash);

            var createdUser = await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<UserDto>(createdUser);
            result.PasswordHash = ""; // Don't return password hash
            return result;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return _jwtTokenService.GenerateToken(user);
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return null;

            var userDto = _mapper.Map<UserDto>(user);
            userDto.PasswordHash = ""; // Don't return password hash
            return userDto;
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}
