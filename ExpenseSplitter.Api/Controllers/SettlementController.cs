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
[Route("api/settlements")]
public class SettlementsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SettlementsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    [ProducesResponseType(typeof(SettlementResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SettlementResponse>> Create(CreateSettlementRequest request)
    {
        if (request.FromUserId == request.ToUserId)
        {
            return BadRequest(new { message = "FromUserId and ToUserId cannot be the same." });
        }

        var groupExists = await _db.Groups.AnyAsync(g => g.Id == request.GroupId);
        if (!groupExists)
        {
            return NotFound(new { message = "Group not found." });
        }

        var groupMemberIds = await _db.GroupMembers
            .Where(gm => gm.GroupId == request.GroupId)
            .Select(gm => gm.UserId)
            .ToListAsync();

        if (!groupMemberIds.Contains(request.FromUserId) || !groupMemberIds.Contains(request.ToUserId))
        {
            return BadRequest(new { message = "Both users must be members of the group." });
        }

        var fromUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.FromUserId);
        var toUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.ToUserId);

        if (fromUser is null || toUser is null)
        {
            return BadRequest(new { message = "One or both users do not exist." });
        }

        var settlement = new Settlement
        {
            GroupId = request.GroupId,
            FromUserId = request.FromUserId,
            ToUserId = request.ToUserId,
            Amount = request.Amount,
            CreatedAt = DateTime.UtcNow
        };

        _db.Settlements.Add(settlement);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByGroup), new { groupId = request.GroupId }, new SettlementResponse
        {
            Id = settlement.Id,
            GroupId = settlement.GroupId,
            FromUserId = settlement.FromUserId,
            FromUserEmail = fromUser.Email,
            ToUserId = settlement.ToUserId,
            ToUserEmail = toUser.Email,
            Amount = settlement.Amount,
            CreatedAt = settlement.CreatedAt
        });
    }

    [HttpGet("group/{groupId:guid}")]
    [ProducesResponseType(typeof(List<SettlementResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<SettlementResponse>>> GetByGroup(Guid groupId)
    {
        var groupExists = await _db.Groups.AnyAsync(g => g.Id == groupId);
        if (!groupExists)
        {
            return NotFound(new { message = "Group not found." });
        }

        var settlements = await _db.Settlements
            .AsNoTracking()
            .Where(s => s.GroupId == groupId)
            .Include(s => s.FromUser)
            .Include(s => s.ToUser)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SettlementResponse
            {
                Id = s.Id,
                GroupId = s.GroupId,
                FromUserId = s.FromUserId,
                FromUserEmail = s.FromUser.Email,
                ToUserId = s.ToUserId,
                ToUserEmail = s.ToUser.Email,
                Amount = s.Amount,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync();

        return Ok(settlements);
    }
}