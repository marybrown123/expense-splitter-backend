using ExpenseSplitter.Api.DTOs.Requests;
using ExpenseSplitter.Api.DTOs.Responses;
using ExpenseSplitter.Application.Entities;
using ExpenseSplitter.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Api.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _db;

    public GroupsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupResponse>> Create(CreateGroupRequest request)
    {
        var ownerExists = await _db.Users.AnyAsync(u => u.Id == request.OwnerId);
        if (!ownerExists)
        {
            return BadRequest(new { message = "Owner user does not exist." });
        }

        var group = new Group
        {
            Name = request.Name.Trim(),
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "PLN" : request.Currency.Trim().ToUpper(),
            OwnerId = request.OwnerId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Groups.Add(group);

        _db.GroupMembers.Add(new GroupMember
        {
            GroupId = group.Id,
            UserId = request.OwnerId,
            JoinedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = group.Id }, new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Currency = group.Currency,
            OwnerId = group.OwnerId,
            CreatedAt = group.CreatedAt
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GroupResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GroupResponse>>> GetAll()
    {
        var groups = await _db.Groups
            .AsNoTracking()
            .OrderByDescending(g => g.CreatedAt)
            .Select(g => new GroupResponse
            {
                Id = g.Id,
                Name = g.Name,
                Currency = g.Currency,
                OwnerId = g.OwnerId,
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
            CreatedAt = group.CreatedAt
        });
    }

    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(typeof(GroupMemberResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupMemberResponse>> AddMember(Guid id, AddGroupMemberRequest request)
    {
        var group = await _db.Groups.FirstOrDefaultAsync(g => g.Id == id);
        if (group is null)
        {
            return NotFound(new { message = "Group not found." });
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user is null)
        {
            return BadRequest(new { message = "User does not exist." });
        }

        var alreadyMember = await _db.GroupMembers
            .AnyAsync(gm => gm.GroupId == id && gm.UserId == request.UserId);

        if (alreadyMember)
        {
            return BadRequest(new { message = "User is already a member of this group." });
        }

        var member = new GroupMember
        {
            GroupId = id,
            UserId = request.UserId,
            JoinedAt = DateTime.UtcNow
        };

        _db.GroupMembers.Add(member);
        await _db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new GroupMemberResponse
        {
            GroupId = member.GroupId,
            UserId = user.Id,
            Email = user.Email,
            Role = member.Role.ToString(),
            JoinedAt = member.JoinedAt
        });
    }
}