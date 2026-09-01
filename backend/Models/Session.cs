using System.ComponentModel.DataAnnotations.Schema;

namespace GraphForge.Api.Models;

[Table("sessions")]
public class Session
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string RefreshTokenHash { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}
