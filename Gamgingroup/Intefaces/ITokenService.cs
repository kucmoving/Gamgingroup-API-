using Gamgingroup.Models;

namespace Gamgingroup.Intefaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
