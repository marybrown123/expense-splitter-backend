using System.ComponentModel.DataAnnotations;

namespace ExpenseSplitter.Api.DTOs.Requests;

public class AddGroupMemberRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}