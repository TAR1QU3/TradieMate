using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TradieMateWeb.API.Data;
using TradieMateWeb.API.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
        .Include(j => j.InvoiceItems)
        .OrderByDescending(j => j.JobDate)
        .ToListAsync();
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Job job)
    {
        var existing = await _db.Jobs
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == GetUserId());
        if (existing == null) return NotFound();

        existing.Title = job.Title;
        existing.Description = job.Description;
        existing.Status = job.Status;
        existing.DueDate = job.DueDate.ToUniversalTime();
        existing.PaymentTerms = job.PaymentTerms;

        await _db.SaveChangesAsync();
        return Ok(existing);
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
    [HttpGet("{id}/invoice")]
    public async Task<IActionResult> GenerateInvoice(int id)
    {
        var job = await _db.Jobs
            .Include(j => j.Client)
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == GetUserId());

        if (job == null) return NotFound();

        var settings = await _db.BusinessSettings
            .FirstOrDefaultAsync(s => s.Id == GetUserId());

        settings ??= new BusinessSettings();

        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        var jobWithItems = await _db.Jobs
            .Include(j => j.InvoiceItems)
            .Include(j => j.Client)
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == GetUserId());

        if (jobWithItems == null) return NotFound();
        job = jobWithItems;

        var gstRate = settings.GSTRate > 0 ? settings.GSTRate / 100 : 0.10;
        var subtotal = job.InvoiceItems.Sum(i => i.Amount);
        var gst = subtotal * gstRate;
        var total = subtotal + gst;
        var pdf = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(QuestPDF.Helpers.PageSizes.A4);
                page.Margin(40);
                page.PageColor(QuestPDF.Helpers.Colors.White);

                page.Content().Column(col =>
                {
                    // Header
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text(string.IsNullOrEmpty(settings.BusinessName)
                                ? "TradieMate" : settings.BusinessName)
                                .FontSize(26).Bold().FontColor("#1E1E2E");

                            if (!string.IsNullOrEmpty(settings.ABN))
                                c.Item().Text($"ABN: {settings.ABN}")
                                    .FontSize(11).FontColor("#6C7086");

                            if (!string.IsNullOrEmpty(settings.Phone))
                                c.Item().Text($"Phone: {settings.Phone}")
                                    .FontSize(11).FontColor("#6C7086");

                            if (!string.IsNullOrEmpty(settings.Email))
                                c.Item().Text($"Email: {settings.Email}")
                                    .FontSize(11).FontColor("#6C7086");
                        });

                        row.ConstantItem(160).Column(c =>
                        {
                            c.Item().Text("TAX INVOICE")
                                .FontSize(20).Bold().FontColor("#89B4FA").AlignRight();
                            c.Item().Text($"#{job.Id:D4}")
                                .FontSize(14).FontColor("#6C7086").AlignRight();
                            c.Item().Text($"Date: {DateTime.UtcNow:dd MMM yyyy}")
                                .FontSize(10).FontColor("#6C7086").AlignRight();
                            c.Item().Text($"Due: {job.DueDate:dd MMM yyyy}")
                                .FontSize(10).FontColor("#F38BA8").AlignRight();
                        });
                    });

                    col.Item().PaddingVertical(12).LineHorizontal(1).LineColor("#E0E0E0");

                    // Bill To
                    col.Item().PaddingBottom(12).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("BILL TO").FontSize(10).Bold().FontColor("#6C7086");
                            c.Item().Text(job.Client?.Name ?? job.ClientId.ToString())
                                .FontSize(14).Bold();
                        });

                        row.ConstantItem(200).Column(c =>
                        {
                            c.Item().Text("JOB DETAILS").FontSize(10).Bold().FontColor("#6C7086");
                            c.Item().Text(job.Title).FontSize(12).Bold();
                            if (!string.IsNullOrEmpty(job.Description))
                                c.Item().Text(job.Description).FontSize(10).FontColor("#6C7086");
                            c.Item().Text($"Status: {job.Status}").FontSize(10).FontColor("#6C7086");
                        });
                    });

                    col.Item().PaddingVertical(4).LineHorizontal(1).LineColor("#E0E0E0");

                    // Table
                    col.Item().PaddingVertical(12).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background("#1E1E2E").Padding(8)
                                .Text("Description").FontColor(QuestPDF.Helpers.Colors.White).Bold();
                            header.Cell().Background("#1E1E2E").Padding(8)
                                .Text("Amount (AUD)").FontColor(QuestPDF.Helpers.Colors.White).Bold().AlignRight();
                        });

                        foreach (var item in job.InvoiceItems)
                        {
                            table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(8).Text(item.Description);
                            table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(8).Text($"${item.Amount:N2}").AlignRight();
                        }

                        table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(8).Text("GST (10%)").FontColor("#6C7086");
                        table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(8).Text($"${gst:N2}").AlignRight().FontColor("#6C7086");

                        table.Cell().Background("#F0F4FF").Padding(10).Text("TOTAL DUE (AUD)").Bold().FontSize(14);
                        table.Cell().Background("#F0F4FF").Padding(10).Text($"${total:N2}").Bold().FontSize(14).AlignRight().FontColor("#89B4FA");
                    });

                    col.Item().PaddingVertical(12).LineHorizontal(1).LineColor("#E0E0E0");

                    // Payment Details
                    if (!string.IsNullOrEmpty(settings.PayID) || !string.IsNullOrEmpty(settings.BankName))
                    {
                        col.Item().PaddingVertical(8).Background("#F8F9FF").Padding(12).Column(c =>
                        {
                            c.Item().Text("PAYMENT DETAILS").FontSize(11).Bold().FontColor("#1E1E2E");

                            if (!string.IsNullOrEmpty(settings.PayID))
                                c.Item().Text($"PayID: {settings.PayID}").FontSize(11);

                            if (!string.IsNullOrEmpty(settings.BankName))
                                c.Item().Text($"Bank: {settings.BankName}").FontSize(11);

                            if (!string.IsNullOrEmpty(settings.BSB))
                                c.Item().Text($"BSB: {settings.BSB}  Account: {settings.AccountNumber}").FontSize(11);

                            c.Item().PaddingTop(4).Text($"Reference: Invoice #{job.Id:D4}")
                                .FontSize(10).FontColor("#6C7086").Italic();
                        });
                    }

                    col.Item().PaddingTop(8).Text(job.IsPaid ? "PAID" : "PAYMENT DUE")
                        .FontSize(14).Bold()
                        .FontColor(job.IsPaid ? "#A6E3A1" : "#F38BA8");

                    col.Item().PaddingTop(16).LineHorizontal(1).LineColor("#E0E0E0");
                    col.Item().PaddingTop(8).Text(
                        string.IsNullOrEmpty(settings.InvoiceNotes)
                        ? "Thank you for your business!"
                        : settings.InvoiceNotes)
                        .FontSize(10).FontColor("#6C7086").AlignCenter();

                    if (!settings.IsPro)
                    {
                        col.Item().PaddingTop(4).Text("Generated by TradieMate - tradiemate-client.onrender.com")
                            .FontSize(9).FontColor("#AAAAAA").AlignCenter();
                    }
                });
            });
        }).GeneratePdf();

        return File(pdf, "application/pdf", $"Invoice_{job.Id:D4}.pdf");
    }
}