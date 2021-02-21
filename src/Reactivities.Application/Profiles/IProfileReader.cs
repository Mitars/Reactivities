using System.Threading.Tasks;

namespace Reactivities.Application.Profiles
{
    public interface IProfileReader
    {
        Task<Profile> ReadProfile(string username);
    }
}