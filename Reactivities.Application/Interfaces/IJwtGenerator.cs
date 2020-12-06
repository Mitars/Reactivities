using Reactivities.Domain;

namespace Reactivities.Application.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(AppUser user);
    }
}