namespace ExpenseSplitter.Api.DTOs.Requests;

public class CreateExpenseParticipantRequest
{
    public Guid UserId { get; set; }
    public decimal ShareAmount { get; set; }
}