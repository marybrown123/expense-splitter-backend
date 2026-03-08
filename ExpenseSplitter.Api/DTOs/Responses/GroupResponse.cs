namespace ExpenseSplitter.Api.DTOs.Responses;

public class GroupResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Currency { get; set; } = "PLN";
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}