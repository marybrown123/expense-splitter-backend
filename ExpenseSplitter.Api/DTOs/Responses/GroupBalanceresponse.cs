namespace ExpenseSplitter.Api.DTOs.Responses;

public class GroupBalanceResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = default!;
    public decimal Paid { get; set; }
    public decimal Owed { get; set; }
    public decimal Balance { get; set; }
}