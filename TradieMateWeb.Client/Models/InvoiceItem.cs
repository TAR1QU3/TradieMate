namespace TradieMateWeb.Client.Models;

public class InvoiceItem
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Quantity { get; set; } = 1;
    public double UnitPrice { get; set; }
    public double Amount => Quantity * UnitPrice;
}