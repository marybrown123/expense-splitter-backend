using System.ComponentModel.DataAnnotations;

namespace ExpenseSplitter.Api.DTOs.Requests;

public class AddGroupMemberRequest
{
    [Required]
    public Guid UserId { get; set; }
}