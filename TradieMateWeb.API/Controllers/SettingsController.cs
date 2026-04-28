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
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SettingsController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var settings = await _db.BusinessSettings
            .FirstOrDefaultAsync(s => s.Id == GetUserId());
        return Ok(settings ?? new BusinessSettings());
    }

    [HttpPost]
    public async Task<IActionResult> Save(BusinessSettings settings)
    {
        var userId = GetUserId();
        settings.Id = userId;
        var existing = await _db.BusinessSettings.FindAsync(userId);
        if (existing == null)
            _db.BusinessSettings.Add(settings);
        else
        {
            existing.BusinessName = settings.BusinessName;
            existing.OwnerName = settings.OwnerName;
            existing.ABN = settings.ABN;
            existing.Phone = settings.Phone;
            existing.Email = settings.Email;
            existing.Address = settings.Address;
            existing.BankName = settings.BankName;
            existing.BSB = settings.BSB;
            existing.AccountNumber = settings.AccountNumber;
            existing.PayID = settings.PayID;
            existing.PaymentTerms = settings.PaymentTerms;
            existing.InvoiceNotes = settings.InvoiceNotes;
            existing.GSTRate = settings.GSTRate;
            existing.IsPro = settings.IsPro;
        }
        await _db.SaveChangesAsync();
        return Ok(settings);
    }
}