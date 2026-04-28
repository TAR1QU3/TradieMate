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
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ClientsController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _db.Clients
            .Where(c => c.UserId == GetUserId())
            .OrderBy(c => c.Name)
            .ToListAsync();
        return Ok(clients);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Client client)
    {
        client.UserId = GetUserId();
        client.CreatedAt = DateTime.UtcNow;
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return Ok(client);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Client client)
    {
        var existing = await _db.Clients
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == GetUserId());
        if (existing == null) return NotFound();

        existing.Name = client.Name;
        existing.Phone = client.Phone;
        existing.Email = client.Email;
        existing.Address = client.Address;

        await _db.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _db.Clients
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == GetUserId());
        if (client == null) return NotFound();
        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return Ok();
    }
}