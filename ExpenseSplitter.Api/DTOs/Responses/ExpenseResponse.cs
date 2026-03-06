namespace ExpenseSplitter.Api.DTOs.Responses;

public class ExpenseResponse
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid PaidByUserId { get; set; }

    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public List<ExpenseParticipantResponse> Participants { get; set; } = new();
}