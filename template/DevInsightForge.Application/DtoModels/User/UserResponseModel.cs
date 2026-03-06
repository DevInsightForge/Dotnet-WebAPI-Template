namespace DevInsightForge.Application.DtoModels.User;

public class UserResponseModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateJoined { get; set; }
    public DateTime LastLogin { get; set; }
}


