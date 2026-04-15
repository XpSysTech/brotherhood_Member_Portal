using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
