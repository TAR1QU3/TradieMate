using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradieMateWeb.API.Data;
using TradieMateWeb.API.Models;

namespace TradieMateWeb.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SettingsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var settings = await _db.BusinessSettings.FirstOrDefaultAsync();
        return Ok(settings ?? new BusinessSettings());
    }

    [HttpPost]
    public async Task<IActionResult> Save(BusinessSettings settings)
    {
        settings.Id = 1;
        var existing = await _db.BusinessSettings.FindAsync(1);
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
        }
        await _db.SaveChangesAsync();
        return Ok(settings);
    }
}