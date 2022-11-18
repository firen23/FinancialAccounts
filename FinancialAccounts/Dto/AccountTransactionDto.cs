using System.ComponentModel.DataAnnotations;

namespace FinancialAccounts.Dto;

public class AccountTransactionDto
{
    /// <summary>
    /// 
    /// </summary>
    public long ClientId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [Range(0.01, float.MaxValue, ErrorMessage = "Sum must be positive")]
    [DataType(DataType.Currency, ErrorMessage = "Must be a Decimal!")]
    public decimal Sum { get; set; }
}
