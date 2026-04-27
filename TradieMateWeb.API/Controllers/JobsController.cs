using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TradieMateWeb.API.Data;
using TradieMateWeb.API.Models;

namespace TradieMateWeb.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly AppDbContext _db;

    public JobsController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var jobs = await _db.Jobs
            .Where(j => j.UserId == GetUserId())
            .Include(j => j.Client)
            .OrderByDescending(j => j.JobDate)
            .ToListAsync();
        return Ok(jobs);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Job job)
    {
        job.UserId = GetUserId();
        job.JobDate = DateTime.UtcNow;
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();
        return Ok(job);
    }

    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var job = await _db.Jobs
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == GetUserId());
        if (job == null) return NotFound();
        job.IsPaid = true;
        await _db.SaveChangesAsync();
        return Ok(job);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _db.Jobs
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == GetUserId());
        if (job == null) return NotFound();
        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();
        return Ok();
    }
}