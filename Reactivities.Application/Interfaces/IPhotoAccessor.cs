using Microsoft.AspNetCore.Http;
using Reactivities.Application.Photos;

namespace Reactivities.Application.Interfaces
{
    public interface IPhotoAccessor
    {
        PhotoUploadResult AddPhoto(IFormFile file);
        string DeletePhoto(string publicId);
    }
}