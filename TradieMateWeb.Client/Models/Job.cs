using System;

namespace TradieMateWeb.Client.Models;

public class Job
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double LaborCost { get; set; }
    public double MaterialCost { get; set; }
    public double TotalAmount => LaborCost + MaterialCost;
    public string Status { get; set; } = "Pending";
    public DateTime JobDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(14);
    public bool IsPaid { get; set; } = false;
    public string PaymentTerms { get; set; } = "Net 14";
}