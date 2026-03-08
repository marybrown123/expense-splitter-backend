namespace ExpenseSplitter.Application.Entities;

public class Group
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Currency { get; set; } = "PLN";
    public Guid OwnerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<GroupMember> Members { get; set; } = new();
    public List<Expense> Expenses { get; set; } = new();
    public List<Settlement> Settlements { get; set; } = new();
    public User Owner { get; set; } = default!;
}