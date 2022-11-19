using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialAccounts.Models;

public class AccountTransaction
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public string Guid { get; set; }
    
    public long AccountId { get; set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime Timestamp { get; set; }
    
    [Timestamp]
    public byte[]? Version { get; set; }
    
    public decimal Sum { get; set; }
    
    public TransactionState State { get; set; }
    
    public Account Account { get; set; }
}
