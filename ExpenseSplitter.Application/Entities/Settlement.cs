namespace ExpenseSplitter.Application.Entities;

public class Settlement
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = default!;

    public Guid FromUserId { get; set; }
    public User FromUser { get; set; } = default!;

    public Guid ToUserId { get; set; }
    public User ToUser { get; set; } = default!;

    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}