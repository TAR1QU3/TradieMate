using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradieMateWeb.API.Data;
using TradieMateWeb.API.Models;

namespace TradieMateWeb.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ClientsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _db.Clients.OrderBy(c => c.Name).ToListAsync();
        return Ok(clients);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Client client)
    {
        client.CreatedAt = DateTime.Now;
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return Ok(client);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null) return NotFound();
        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return Ok();
    }
}