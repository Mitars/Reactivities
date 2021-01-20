using System;

namespace Reactivities.Domain
{
    public class RefreshToken
    {
        public int Id { get; init; }
        public virtual AppUser Appuser { get; init; }
        public string Token { get; init; }
        public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(7);
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
