namespace Reactivities.Application.Activities
{
    public record AttendeeDto
    {
        public string UserName { get; init; }
        public string DisplayName { get; init; }
        public string Image { get; init; }
        public bool IsHost { get; init; }
        public bool Following { get; set; }
        
        
    }
}
