using System.ComponentModel.DataAnnotations;

namespace ExpenseSplitter.Api.DTOs.Requests;

public class CreateGroupRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Currency { get; set; } = "PLN";
}