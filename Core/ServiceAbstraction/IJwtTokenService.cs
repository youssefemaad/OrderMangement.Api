using OrderManagement.Core.DomainLayer.Entities;

namespace OrderManagement.Core.ServiceAbstraction
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }
}
