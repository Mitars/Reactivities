namespace Reactivities.Infrastructure.Email
{
    public record SendGridSettings
    {
        public string User { get; init; }
        public string Key { get; init; }
    }
}