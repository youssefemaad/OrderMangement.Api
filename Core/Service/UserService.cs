using AutoMapper;
using OrderManagement.Core.DomainLayer.Contracts;
using OrderManagement.Core.DomainLayer.Entities;
using OrderManagement.Core.ServiceAbstraction;
using OrderMangement.Api.Shared.DataTransferObject;
using BCrypt.Net;

namespace OrderManagement.Core.Service
{
    public class UserService(IUnitOfWork unitOfWork, IMapper mapper, IJwtTokenService jwtTokenService) : IUserService
    {
        public async Task<UserDto> RegisterUserAsync(UserDto userDto)
        {
            var userRepo = unitOfWork.GetRepository<User, int>();
            var existingUsers = await userRepo.FindAsync(u => u.Username == userDto.Username);
            if (existingUsers.Any())
                throw new InvalidOperationException("Username already exists");

            var user = mapper.Map<User>(userDto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash);

            var createdUser = await userRepo.AddAsync(user);
            await unitOfWork.SaveChangesAsync();

            var result = mapper.Map<UserDto>(createdUser);
            result.PasswordHash = "";
            return result;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var userRepo = unitOfWork.GetRepository<User, int>();
            var users = await userRepo.FindAsync(u => u.Username == username);
            var user = users.FirstOrDefault();

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return jwtTokenService.GenerateToken(user);
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var userRepo = unitOfWork.GetRepository<User, int>();
            var user = await userRepo.GetByIdAsync(userId);
            if (user == null)
                return null;

            var userDto = mapper.Map<UserDto>(user);
            userDto.PasswordHash = "";
            return userDto;
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            var userRepo = unitOfWork.GetRepository<User, int>();
            var users = await userRepo.FindAsync(u => u.Username == username);
            var user = users.FirstOrDefault();

            return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}
