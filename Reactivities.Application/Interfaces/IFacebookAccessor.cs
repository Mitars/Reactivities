using System.Threading.Tasks;
using Reactivities.User;

namespace Reactivities.Application.Interfaces
{
    public interface IFacebookAccessor
    {
        Task<FacebookUserInfo> FacebookLogin(string accessToken);
    }
}