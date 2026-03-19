namespace DevInsightForge.Application.Contracts.User;

public class UserResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string RoleId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateJoined { get; set; }
    public DateTime LastLogin { get; set; }
}






