namespace ExpenseSplitter.Api.DTOs.Responses;

public class SettlementSuggestionResponse
{
    public Guid FromUserId { get; set; }
    public string FromUsername { get; set; } = default!;
    public Guid ToUserId { get; set; }
    public string ToUsername { get; set; } = default!;
    public decimal Amount { get; set; }
}