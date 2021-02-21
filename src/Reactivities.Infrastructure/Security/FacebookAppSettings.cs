namespace Reactivities.Infrastructure.Security
{
    public record FacebookAppSettings
    {
        public string AppId { get; init; }
        public string AppSecret { get; init; }
    }
}