using Microsoft.EntityFrameworkCore;
using FinancialAccounts.Data;
using FinancialAccounts.Dto;
using FinancialAccounts.Models;
using IsolationLevel = System.Data.IsolationLevel;

namespace FinancialAccounts.Services;

public class AccountService : IAccountService
{
    private readonly IDbContextFactory<FinancialAccountsContext> _contextFactory;
    private const int UpdateMaxAttempts = 10;

    public AccountService(IDbContextFactory<FinancialAccountsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<ClientBalanceDto> GetBalance(long clientId)
    {
        ClientBalanceDto clientBalanceDto;
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var account = await context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.ClientId == clientId);

            if (account is null)
            {
                throw (new KeyNotFoundException($"Client with id {clientId} not found"));
            }

            clientBalanceDto = new ClientBalanceDto
            {
                ClientId = clientId,
                Balance = account.Balance
            };
        }

        return clientBalanceDto;
    }

    public async Task Accrue(AccountTransactionDto accountTransactionDto)
    {
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var account = await context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.ClientId == accountTransactionDto.ClientId);

            if (account is null)
            {
                throw new KeyNotFoundException($"Client id {accountTransactionDto.ClientId} not found");
            }

            var transactionGuid = Guid.NewGuid();

            var accountTransaction = new AccountTransaction
            {
                AccountId = account.Id,
                Guid = transactionGuid.ToString(),
                Sum = Math.Abs(accountTransactionDto.Sum),
                Timestamp = DateTime.UtcNow,
                State = TransactionState.Pending
            };

            AddAccountTransaction(context, accountTransaction);
            AcceptPendingTransactions(context, account.Id);

            var transactionState = GetTransactionState(context, accountTransaction);
            if (transactionState == TransactionState.Rejected)
            {
                throw new DbUpdateException($"Client id {accountTransactionDto.ClientId} " +
                                            $"has insufficient funds");
            }
        }
    }

    public async Task Debit(AccountTransactionDto accountTransactionDto)
    {
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var account = await context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.ClientId == accountTransactionDto.ClientId);

            if (account is null)
            {
                throw new KeyNotFoundException($"Client id {accountTransactionDto.ClientId} not found");
            }

            var transactionGuid = Guid.NewGuid();

            var accountTransaction = new AccountTransaction
            {
                AccountId = account.Id,
                Guid = transactionGuid.ToString(),
                Sum = (-1) * Math.Abs(accountTransactionDto.Sum),
                Timestamp = DateTime.UtcNow,
                State = TransactionState.Pending
            };

            AddAccountTransaction(context, accountTransaction);
            AcceptPendingTransactions(context, account.Id);

            var transactionState = GetTransactionState(context, accountTransaction);
            if (transactionState == TransactionState.Rejected)
            {
                throw new DbUpdateException($"Client id {accountTransactionDto.ClientId} " +
                                            $"has insufficient funds");
            }
        }
    }

    private void AddAccountTransaction(FinancialAccountsContext context, AccountTransaction accountTransaction)
    {
        using (var dbTransaction = context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
        {
            try
            {

                context.Entry(accountTransaction).State = EntityState.Added;
                context.AccountTransactions.Add(accountTransaction);
                context.SaveChanges();
                dbTransaction.Commit();
            }
            catch (Exception e)
            {
                dbTransaction.Rollback();
                throw;
            }
        }
    }

    private void AcceptPendingTransactions(FinancialAccountsContext context, long accountId)
    {
        var transaction = GetNextPendingTransaction(context, accountId);
        var updateAttempts = 0;

        while (transaction is not null && updateAttempts < UpdateMaxAttempts)
        {
            using (var dbTransaction = context.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var account = context.Accounts
                        .FirstOrDefault(acc => acc.Id == transaction.AccountId);
        
                    if (account is null)
                    {
                        throw new KeyNotFoundException($"Account id {transaction.AccountId} not found");
                    }
                    
                    var newBalance = account.Balance + transaction.Sum;

                    if (newBalance < 0)
                    {
                        transaction.State = TransactionState.Rejected;
                    }
                    else
                    {
                        account.Balance = newBalance;
                        account.LastTransactionId = transaction.Id;
                        transaction.State = TransactionState.Accepted;
                    }

                    context.SaveChanges();
                    dbTransaction.Commit();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    // Transaction already processed
                    dbTransaction.Rollback();
                    HandleDbUpdateConcurrencyException(e);
                    updateAttempts++;
                }
                catch (Exception e)
                {
                    dbTransaction.Rollback();
                    // Some other exception
                    throw new DbUpdateException($"Failed to debit {transaction.Sum} " +
                                                $"to account id {transaction.AccountId}: {e.Message}");
                }
            }
            
            transaction = GetNextPendingTransaction(context, accountId);
        }

        if (updateAttempts == UpdateMaxAttempts)
        {
            throw new DbUpdateConcurrencyException($"Fail to apply accountTransaction id {transaction.Id}");
        }
    }

    private AccountTransaction? GetNextPendingTransaction(FinancialAccountsContext context, long accountId)
    {
        return context.AccountTransactions
            .Where(transaction => transaction.AccountId == accountId && transaction.State == TransactionState.Pending)
            .OrderBy(transaction => transaction.Id)
            .FirstOrDefault();
    }

    private TransactionState GetTransactionState(FinancialAccountsContext context, AccountTransaction accountTransaction)
    {
        var transaction = context.AccountTransactions
            .AsNoTracking()
            .FirstOrDefault(transaction => transaction.AccountId == accountTransaction.AccountId &&
                                           transaction.Guid.Equals(accountTransaction.Guid) &&
                                           transaction.Sum == accountTransaction.Sum);

        if (transaction is null)
        {
            throw new ArgumentException($"Unable to find transaction " +
                                        $"| account id {accountTransaction.AccountId}, guid {accountTransaction.Guid}");
        }
        
        return transaction.State;
    }

    private void HandleDbUpdateConcurrencyException(DbUpdateConcurrencyException exception)
    {
        foreach (var entry in exception.Entries)
        {
            if (entry.Entity is Account or AccountTransaction)
            {
                var proposedValues = entry.CurrentValues;
                var databaseValues = entry.GetDatabaseValues();
                        
                foreach (var property in proposedValues.Properties)
                {
                    var databaseValue = databaseValues[property];
                    proposedValues[property] = databaseValue;
                }

                // Refresh original values to bypass next concurrency check
                entry.OriginalValues.SetValues(databaseValues);
            }
            else
            {
                throw new NotSupportedException(
                    $"Don't know how to handle concurrency conflicts for {entry.Metadata.Name}");
            }
        }
    }
}
