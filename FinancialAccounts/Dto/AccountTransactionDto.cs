using System.ComponentModel.DataAnnotations;

namespace FinancialAccounts.Dto;

public class AccountTransactionDto
{
    /// <summary>
    /// ClientId
    /// </summary>
    public long ClientId { get; set; }
    
    /// <summary>
    /// Transaction sum
    /// </summary>
    [Range(0.01d, double.MaxValue, ErrorMessage = "Sum must be positive")]
    [DataType(DataType.Currency, ErrorMessage = "Must be a Decimal!")]
    public decimal Sum { get; set; }
}
