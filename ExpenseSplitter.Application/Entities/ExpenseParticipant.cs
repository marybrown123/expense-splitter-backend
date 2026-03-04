namespace ExpenseSplitter.Application.Entities;

public class ExpenseParticipant
{
    public Guid ExpenseId { get; set; }
    public Expense Expense { get; set; } = default!;

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public decimal ShareAmount { get; set; }
}