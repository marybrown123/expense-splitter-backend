using ExpenseSplitter.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseParticipant> ExpenseParticipants => Set<ExpenseParticipant>();
    public DbSet<Settlement> Settlements => Set<Settlement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Group>()
            .HasOne(g => g.Owner)
            .WithMany()
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);        

        modelBuilder.Entity<GroupMember>()
            .HasKey(x => new { x.GroupId, x.UserId });

        modelBuilder.Entity<GroupMember>()
            .HasOne(x => x.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(x => x.GroupId);

        modelBuilder.Entity<GroupMember>()
            .HasOne(x => x.User)
            .WithMany(u => u.Groups)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExpenseParticipant>()
            .HasKey(x => new { x.ExpenseId, x.UserId });

        modelBuilder.Entity<ExpenseParticipant>()
            .HasOne(x => x.Expense)
            .WithMany(e => e.Participants)
            .HasForeignKey(x => x.ExpenseId);

        modelBuilder.Entity<ExpenseParticipant>()
            .HasOne(x => x.User)
            .WithMany(u => u.ExpenseParticipations)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Expense>()
            .HasOne(x => x.Group)
            .WithMany(g => g.Expenses)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Expense>()
            .HasOne(x => x.PaidByUser)
            .WithMany(u => u.PaidExpenses)
            .HasForeignKey(x => x.PaidByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Settlement>()
            .HasOne(x => x.Group)
            .WithMany(g => g.Settlements)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Settlement>()
            .HasOne(x => x.FromUser)
            .WithMany(u => u.SettlementsSent)
            .HasForeignKey(x => x.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Settlement>()
            .HasOne(x => x.ToUser)
            .WithMany(u => u.SettlementsReceived)
            .HasForeignKey(x => x.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}