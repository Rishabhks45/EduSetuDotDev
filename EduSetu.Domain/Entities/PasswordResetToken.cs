namespace EduSetu.Domain.Entities;

/// <summary>
/// Password reset token entity for managing secure password reset requests
/// Clean entity with only properties - business logic moved to command handlers
/// </summary>
public class PasswordResetToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string ResetToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;

    // Parameterless constructor for EF Core
    public PasswordResetToken() { }

    /// <summary>
    /// Creates a new password reset token
    /// </summary>
    public PasswordResetToken(Guid userId, string resetToken, DateTime expiresAt)
    {
        UserId = userId;
        ResetToken = resetToken ?? string.Empty;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.Now;
    }
}
