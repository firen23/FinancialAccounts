using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinancialAccounts.Dto;

namespace FinancialAccounts.Models;

public class Account
{
    public long Id { get; set; }
    public long ClientId { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }
    [Timestamp]
    public byte[]? Version { get; set; }
    public Client? Client { get; set; }
}
