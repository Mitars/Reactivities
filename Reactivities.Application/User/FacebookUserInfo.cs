namespace Reactivities.User
{
    public record FacebookUserInfo
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public FacebookPictureData Picture { get; init; }
        public string PictureUrl => this.Picture.Data.Url;
    }

    public record FacebookPictureData
    {
        public FacebookPicture Data { get; init; }
    }

    public record FacebookPicture
    {
        public string Url { get; init; }
    }
}