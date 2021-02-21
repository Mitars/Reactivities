namespace Reactivities.Application.Photos
{
    public record PhotoUploadResult
    {
        public string PublicId { get; init; }
        public string Url { get; init; }
    }
}