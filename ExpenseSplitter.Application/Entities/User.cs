namespace ExpenseSplitter.Application.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Username { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<GroupMember> Groups { get; set; } = new();
    public List<Expense> PaidExpenses { get; set; } = new();
    public List<ExpenseParticipant> ExpenseParticipations { get; set; } = new();

    public List<Settlement> SettlementsSent { get; set; } = new();
    public List<Settlement> SettlementsReceived { get; set; } = new();
}