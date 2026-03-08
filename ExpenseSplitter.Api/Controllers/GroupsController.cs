using ExpenseSplitter.Api.DTOs.Requests;
using ExpenseSplitter.Api.DTOs.Responses;
using ExpenseSplitter.Application.Entities;
using ExpenseSplitter.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ExpenseSplitter.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/groups")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _db;

    private async Task<bool> IsOwner(Guid groupId, Guid userId)
    {
        return await _db.GroupMembers
            .AnyAsync(gm =>
                gm.GroupId == groupId &&
                gm.UserId == userId &&
                gm.Role == GroupRole.Owner);
    }

    public GroupsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<GroupResponse>> Create(CreateGroupRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var group = new Group
        {
            Name = request.Name,
            Currency = request.Currency,
            OwnerId = userId
        };

        var owner = await _db.Users
            .AsNoTracking()
            .FirstAsync(u => u.Id == userId);

        _db.Groups.Add(group);

        _db.GroupMembers.Add(new GroupMember
        {
            GroupId = group.Id,
            UserId = userId,
            Role = GroupRole.Owner,
            JoinedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = group.Id }, new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Currency = group.Currency,
            OwnerId = group.OwnerId,
            OwnerName = owner.Username,
            CreatedAt = group.CreatedAt
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GroupResponse>>> GetAll()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var groups = await _db.GroupMembers
            .AsNoTracking()
            .Where(gm => gm.UserId == userId)
            .Select(gm => gm.Group)
            .Distinct()
            .OrderByDescending(g => g.CreatedAt)
            .Select(g => new GroupResponse
            {
                Id = g.Id,
                Name = g.Name,
                Currency = g.Currency,
                OwnerId = g.OwnerId,
                OwnerName = g.Owner.Username,
                CreatedAt = g.CreatedAt
            })
            .ToListAsync();

        return Ok(groups);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupResponse>> GetById(Guid id)
    {
        var group = await _db.Groups
            .AsNoTracking()
            .Include(g => g.Owner)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group is null)
        {
            return NotFound();
        }

        return Ok(new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Currency = group.Currency,
            OwnerId = group.OwnerId,
            OwnerName = group.Owner.Username,
            CreatedAt = group.CreatedAt
        });
    }

    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(typeof(GroupMemberResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupMemberResponse>> AddMember(Guid id, AddGroupMemberRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var isOwner = await IsOwner(id, currentUserId);

        if (!isOwner)
        {
            return Forbid();
        }

        var group = await _db.Groups.FirstOrDefaultAsync(g => g.Id == id);
        if (group is null)
        {
            return NotFound(new { message = "Group not found." });
        }

        var email = request.Email.Trim().ToLower();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            return BadRequest(new { message = "User does not exist." });
        }

        var alreadyMember = await _db.GroupMembers
            .AnyAsync(gm => gm.GroupId == id && gm.UserId == user.Id);
        if (alreadyMember)
        {
            return BadRequest(new { message = "User is already a member of this group." });
        }

        var member = new GroupMember
        {
            GroupId = id,
            UserId = user.Id,
            Role = GroupRole.Member,
            JoinedAt = DateTime.UtcNow
        };

        _db.GroupMembers.Add(member);
        await _db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new GroupMemberResponse
        {
            GroupId = member.GroupId,
            UserId = user.Id,
            Username = user.Username,
            Role = member.Role.ToString(),
            JoinedAt = member.JoinedAt
        });
    }

    [HttpGet("{id:guid}/balances")]
    [ProducesResponseType(typeof(List<GroupBalanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GroupBalanceResponse>>> GetBalances(Guid id)
    {
        var groupExists = await _db.Groups.AnyAsync(g => g.Id == id);
        if (!groupExists)
        {
            return NotFound(new { message = "Group not found." });
        }

        var balances = await CalculateBalances(id);
        return Ok(balances.OrderByDescending(x => x.Balance).ToList());
    }

    [HttpGet("{id:guid}/settlement-suggestions")]
    [ProducesResponseType(typeof(List<SettlementSuggestionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<SettlementSuggestionResponse>>> GetSettlementSuggestions(Guid id)
    {
        var groupExists = await _db.Groups.AnyAsync(g => g.Id == id);
        if (!groupExists)
        {
            return NotFound(new { message = "Group not found." });
        }

        var balances = await CalculateBalances(id);

        var creditors = balances
            .Where(x => x.Balance > 0)
            .Select(x => new SettlementSuggestionItem
            {
                UserId = x.UserId,
                Username = x.Username,
                Amount = x.Balance
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        var debtors = balances
            .Where(x => x.Balance < 0)
            .Select(x => new SettlementSuggestionItem
            {
                UserId = x.UserId,
                Username = x.Username,
                Amount = Math.Abs(x.Balance)
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        var suggestions = new List<SettlementSuggestionResponse>();

        var creditorIndex = 0;
        var debtorIndex = 0;

        while (creditorIndex < creditors.Count && debtorIndex < debtors.Count)
        {
            var creditor = creditors[creditorIndex];
            var debtor = debtors[debtorIndex];

            var transferAmount = decimal.Round(Math.Min(creditor.Amount, debtor.Amount), 2);

            if (transferAmount > 0)
            {
                suggestions.Add(new SettlementSuggestionResponse
                {
                    FromUserId = debtor.UserId,
                    FromUsername = debtor.Username,
                    ToUserId = creditor.UserId,
                    ToUsername = creditor.Username,
                    Amount = transferAmount
                });
            }

            creditor.Amount = decimal.Round(creditor.Amount - transferAmount, 2);
            debtor.Amount = decimal.Round(debtor.Amount - transferAmount, 2);

            if (creditor.Amount == 0)
            {
                creditorIndex++;
            }

            if (debtor.Amount == 0)
            {
                debtorIndex++;
            }
        }

        return Ok(suggestions);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<List<GroupMemberResponse>>> GetMembers(Guid id)
    {
        var members = await _db.GroupMembers
            .Include(gm => gm.User)
            .Where(gm => gm.GroupId == id)
            .Select(gm => new GroupMemberResponse
            {
                GroupId = gm.GroupId,
                UserId = gm.UserId,
                Username = gm.User.Username,
                Role = gm.Role.ToString(),
                JoinedAt = gm.JoinedAt
            })
            .ToListAsync();

        return Ok(members);
    }

    private async Task<List<GroupBalanceResponse>> CalculateBalances(Guid groupId)
    {
        var members = await _db.GroupMembers
            .AsNoTracking()
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => new
            {
                gm.UserId,
                gm.User.Username
            })
            .ToListAsync();

        var expenses = await _db.Expenses
            .AsNoTracking()
            .Include(e => e.Participants)
            .Where(e => e.GroupId == groupId)
            .ToListAsync();

        var settlements = await _db.Settlements
            .AsNoTracking()
            .Where(s => s.GroupId == groupId)
            .ToListAsync();

        var balances = members
            .Select(member =>
            {
                var paid = expenses
                    .Where(e => e.PaidByUserId == member.UserId)
                    .Sum(e => e.Amount);

                var owed = expenses
                    .SelectMany(e => e.Participants)
                    .Where(p => p.UserId == member.UserId)
                    .Sum(p => p.ShareAmount);

                var sentSettlements = settlements
                    .Where(s => s.FromUserId == member.UserId)
                    .Sum(s => s.Amount);

                var receivedSettlements = settlements
                    .Where(s => s.ToUserId == member.UserId)
                    .Sum(s => s.Amount);

                return new GroupBalanceResponse
                {
                    UserId = member.UserId,
                    Username = member.Username,
                    Paid = paid,
                    Owed = owed,
                    Balance = decimal.Round((paid - owed) - sentSettlements + receivedSettlements, 2)
                };
            })
            .ToList();

        return balances;
    }

    private class SettlementSuggestionItem
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public decimal Amount { get; set; }
    }
}