namespace ExpenseSplitter.Api.DTOs.Responses;

public class SettlementResponse
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }

    public Guid FromUserId { get; set; }
    public string FromUserEmail { get; set; } = default!;

    public Guid ToUserId { get; set; }
    public string ToUserEmail { get; set; } = default!;

    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}