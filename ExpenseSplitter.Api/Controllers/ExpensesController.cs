using ExpenseSplitter.Api.DTOs.Requests;
using ExpenseSplitter.Api.DTOs.Responses;
using ExpenseSplitter.Application.Entities;
using ExpenseSplitter.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ExpenseSplitter.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExpensesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetAll()
    {
        var expenses = await _context.Expenses
            .Include(e => e.Participants)
            .ToListAsync();

        var result = expenses.Select(MapToDto).ToList();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> GetById(Guid id)
    {
        var expense = await _context.Expenses
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense is null)
            return NotFound(new { message = "Expense not found." });

        return Ok(MapToDto(expense));
    }

    [HttpGet("group/{groupId:guid}")]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetByGroupId(Guid groupId)
    {
        var expenses = await _context.Expenses
            .Include(e => e.Participants)
            .Where(e => e.GroupId == groupId)
            .ToListAsync();

        var result = expenses.Select(MapToDto).ToList();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create([FromBody] CreateExpenseRequest dto)
    {
        var validationResult = await ValidateExpense(dto.GroupId, dto.PaidByUserId, dto.Amount, dto.Participants);

        if (validationResult is not null)
            return validationResult;

        var expense = new Expense
        {
            GroupId = dto.GroupId,
            Title = dto.Title,
            Amount = dto.Amount,
            PaidByUserId = dto.PaidByUserId,
            ExpenseDate = dto.ExpenseDate ?? DateTime.UtcNow,
            Participants = dto.Participants.Select(p => new ExpenseParticipant
            {
                UserId = p.UserId,
                ShareAmount = p.ShareAmount
            }).ToList()
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        var response = MapToDto(expense);

        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var expense = await _context.Expenses
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense is null)
            return NotFound(new { message = "Expense not found." });

        _context.ExpenseParticipants.RemoveRange(expense.Participants);
        _context.Expenses.Remove(expense);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private ExpenseResponse MapToDto(Expense expense)
    {
        return new ExpenseResponse
        {
            Id = expense.Id,
            GroupId = expense.GroupId,
            Title = expense.Title,
            Amount = expense.Amount,
            PaidByUserId = expense.PaidByUserId,
            ExpenseDate = expense.ExpenseDate,
            Participants = expense.Participants.Select(p => new ExpenseParticipantResponse
            {
                UserId = p.UserId,
                ShareAmount = p.ShareAmount
            }).ToList()
        };
    }

    private async Task<ActionResult?> ValidateExpense(
        Guid groupId,
        Guid paidByUserId,
        decimal amount,
        List<CreateExpenseParticipantRequest> participants)
    {
        if (amount <= 0)
            return BadRequest(new { message = "Amount must be greater than 0." });

        if (participants is null || participants.Count == 0)
            return BadRequest(new { message = "Expense must have at least one participant." });

        var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupId);
        if (!groupExists)
            return BadRequest(new { message = "Group does not exist." });

        var participantUserIds = participants.Select(p => p.UserId).ToList();

        if (participantUserIds.Distinct().Count() != participantUserIds.Count)
            return BadRequest(new { message = "Participants cannot contain duplicate users." });

        if (participants.Any(p => p.ShareAmount < 0))
            return BadRequest(new { message = "ShareAmount cannot be negative." });

        var sharesSum = participants.Sum(p => p.ShareAmount);

        if (decimal.Round(sharesSum, 2) != decimal.Round(amount, 2))
            return BadRequest(new { message = "Sum of participant shares must equal total expense amount." });

        var groupMemberIds = await _context.GroupMembers
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => gm.UserId)
            .ToListAsync();

        if (!groupMemberIds.Contains(paidByUserId))
            return BadRequest(new { message = "PaidByUser must be a member of the group." });

        var invalidParticipantExists = participantUserIds.Any(userId => !groupMemberIds.Contains(userId));
        if (invalidParticipantExists)
            return BadRequest(new { message = "All participants must be members of the group." });

        return null;
    }
}