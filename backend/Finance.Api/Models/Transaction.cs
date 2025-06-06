namespace Finance.Api.Models;

public class Transaction
{
    public Guid UserId { get; set; }
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}