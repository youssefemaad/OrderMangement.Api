using OrderManagement.Core.DomainLayer.Entities;

namespace OrderManagement.Core.DomainLayer.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
    }
}