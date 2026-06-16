using System;

namespace MorgueManager.API.Models;

public class BillingRecord
{
    public int Id { get; set; }
    public int CorpseId { get; set; }
    public double StorageFeePerDay { get; set; } = 200000; // 200,000 VND
    public double ServiceFee { get; set; } = 0;
    public double TotalAmount { get; set; }
    public bool IsPaid { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
