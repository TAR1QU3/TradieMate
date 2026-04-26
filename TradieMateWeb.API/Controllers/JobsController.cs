using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradieMateWeb.API.Data;
using TradieMateWeb.API.Models;

namespace TradieMateWeb.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly AppDbContext _db;

    public JobsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var jobs = await _db.Jobs
            .Include(j => j.Client)
            .OrderByDescending(j => j.JobDate)
            .ToListAsync();
        return Ok(jobs);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Job job)
    {
        job.JobDate = DateTime.Now;
        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();
        return Ok(job);
    }

    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var job = await _db.Jobs.FindAsync(id);
        if (job == null) return NotFound();
        job.IsPaid = true;
        await _db.SaveChangesAsync();
        return Ok(job);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _db.Jobs.FindAsync(id);
        if (job == null) return NotFound();
        _db.Jobs.Remove(job);
        await _db.SaveChangesAsync();
        return Ok();
    }
}