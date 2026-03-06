using System.ComponentModel.DataAnnotations;

namespace ExpenseSplitter.Api.DTOs.Requests;

public class CreateExpenseRequest
{
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public Guid PaidByUserId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public decimal Amount { get; set; }

    public DateTime? ExpenseDate { get; set; }

    [Required]
    public List<CreateExpenseParticipantRequest> Participants { get; set; } = new();
}