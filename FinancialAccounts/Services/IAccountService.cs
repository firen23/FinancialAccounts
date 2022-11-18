using FinancialAccounts.Dto;

namespace FinancialAccounts.Services;

public interface IAccountService
{
    public Task<ClientBalanceDto> GetBalance(long clientId);

    public Task Accrue(AccountTransactionDto accountTransactionDto);

    public Task Debit(AccountTransactionDto accountTransactionDto);
}
