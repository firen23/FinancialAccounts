using System.ComponentModel.DataAnnotations;

namespace FinancialAccounts.Dto;

public class ClientBalanceDto
{
    /// <summary>
    /// Client Id
    /// </summary>
    public long ClientId { get; set; }
    
    /// <summary>
    /// Balance
    /// </summary>
    [DataType(DataType.Currency, ErrorMessage = "Must be a Decimal!")]
    public decimal Balance { get; set; }
}
