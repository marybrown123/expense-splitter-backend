namespace ExpenseSplitter.Api.DTOs.Responses;

public class ExpenseParticipantResponse
{
    public Guid UserId { get; set; }
    public decimal ShareAmount { get; set; }
}