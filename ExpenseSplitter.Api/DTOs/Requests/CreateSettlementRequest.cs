using System.ComponentModel.DataAnnotations;

namespace ExpenseSplitter.Api.DTOs.Requests;

public class CreateSettlementRequest
{
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public Guid FromUserId { get; set; }

    [Required]
    public Guid ToUserId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}