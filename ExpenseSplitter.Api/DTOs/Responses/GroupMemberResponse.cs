namespace ExpenseSplitter.Api.DTOs.Responses;

public class GroupMemberResponse
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}