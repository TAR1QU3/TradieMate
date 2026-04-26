namespace TradieMateWeb.API.Models;

public class BusinessSettings
{
    public int Id { get; set; } = 1;
    public string BusinessName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string ABN { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BSB { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string PayID { get; set; } = string.Empty;
    public string PaymentTerms { get; set; } = "Net 14";
    public string InvoiceNotes { get; set; } = "Thank you for your business!";
}