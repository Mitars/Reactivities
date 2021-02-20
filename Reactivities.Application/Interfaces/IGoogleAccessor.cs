using System.Threading.Tasks;
using Reactivities.User;

namespace Reactivities.Application.Interfaces
{
    public interface IGoogleAccessor
    {
        Task<GoogleUserInfo> Login(string accessToken);
    }
}