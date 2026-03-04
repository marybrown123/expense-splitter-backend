namespace ExpenseSplitter.Application.Entities;

public class Expense
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = default!;

    public string Title { get; set; } = default!;
    public decimal Amount { get; set; }

    public Guid PaidByUserId { get; set; }
    public User PaidByUser { get; set; } = default!;

    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;

    public List<ExpenseParticipant> Participants { get; set; } = new();
}