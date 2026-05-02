using System;
using System.Collections.Generic;
using System.Linq;

namespace TradieMateWeb.Client.Models;

public class Job
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime JobDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);
    public bool IsPaid { get; set; } = false;
    public string PaymentTerms { get; set; } = "Net 14";
    public int UserId { get; set; }
    public List<InvoiceItem> InvoiceItems { get; set; } = new();
    public double TotalAmount => InvoiceItems.Sum(i => i.Amount);
}