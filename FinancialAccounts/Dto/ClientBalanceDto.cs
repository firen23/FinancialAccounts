using System.ComponentModel.DataAnnotations;

namespace FinancialAccounts.Dto;

public class ClientBalanceDto
{
    /// <summary>
    /// 
    /// </summary>
    public long ClientId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    [DataType(DataType.Currency, ErrorMessage = "Must be a Decimal!")]
    public decimal Balance { get; set; }
}
