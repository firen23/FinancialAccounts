using System.Data;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using FinancialAccounts.Data;
using FinancialAccounts.Dto;
using FinancialAccounts.Models;
using IsolationLevel = System.Data.IsolationLevel;

namespace FinancialAccounts.Services;

public class AccountService : IAccountService
{
    private readonly FinancialAccountsContext _context;
    private const int UpdateMaxAttempts = 10;

    public AccountService(FinancialAccountsContext context)
    {
        _context = context;
    }
    
    public async Task<ClientBalanceDto> GetBalance(long clientId)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(account => account.ClientId == clientId);
        
        if (account is null)
        {
            throw (new KeyNotFoundException($"Client with id {clientId} not found"));
        }
        
        var clientBalanceDto = new ClientBalanceDto
        {
            ClientId = clientId,
            Balance = account.Balance
        };
        return clientBalanceDto;
    }

    public async Task Accrue(AccountTransactionDto accountTransactionDto)
    {
        var account = _context.Accounts
                .FirstOrDefault(account => account.ClientId == accountTransactionDto.ClientId);
        
        if (account is null)
        {
            throw new KeyNotFoundException($"Client with id {accountTransactionDto.ClientId} not found");
        }

        account.Balance += accountTransactionDto.Sum;

        var saved = false;
        var attemptCount = 0;

        while (!saved && attemptCount < UpdateMaxAttempts)
        {
            try
            {
                await _context.SaveChangesAsync();
                saved = true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var entry in e.Entries)
                {
                    if (entry.Entity is Account)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = await entry.GetDatabaseValuesAsync();
                        
                        foreach (var property in proposedValues.Properties)
                        {
                            var databaseValue = databaseValues[property];
                            proposedValues[property] = databaseValue;
                        }
                        
                        var currentBalance = databaseValues.GetValue<decimal>("Balance");
                        proposedValues["Balance"] = currentBalance + accountTransactionDto.Sum;

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                        attemptCount++;
                    }
                    else
                    {
                        throw new NotSupportedException(
                            $"Don't know how to handle concurrency conflicts for {entry.Metadata.Name}");
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbUpdateException(
                    $"Failed to accrue {accountTransactionDto.Sum} " +
                    $"to client id {accountTransactionDto.ClientId}: {e.Message}");
            }
        }

        if (attemptCount == UpdateMaxAttempts)
        {
            throw new DbUpdateException(
                $"Failed to accrue {accountTransactionDto.Sum} to client id " +
                $"{accountTransactionDto.ClientId} after {UpdateMaxAttempts} attempts");
        }
    }

    public async Task Debit(AccountTransactionDto accountTransactionDto)
    {
        var account = _context.Accounts
            .FirstOrDefault(account => account.ClientId == accountTransactionDto.ClientId);
        
        if (account is null)
        {
            throw new KeyNotFoundException($"Client id {accountTransactionDto.ClientId} not found");
        }

        if (account.Balance < accountTransactionDto.Sum)
        {
            throw new DbUpdateException($"Client id {accountTransactionDto.ClientId} has insufficient funds");
        }
        
        account.Balance -= accountTransactionDto.Sum;
        
        var saved = false;
        var attemptCount = 0;

        while (!saved && attemptCount < UpdateMaxAttempts)
        {

            try
            {
                await _context.SaveChangesAsync();
                saved = true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var entry in e.Entries)
                {
                    if (entry.Entity is Account)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = await entry.GetDatabaseValuesAsync();
                        
                        var currentBalance = databaseValues.GetValue<decimal>("Balance");
                        var resultBalance = currentBalance - accountTransactionDto.Sum;
                        
                        if (resultBalance < 0)
                        {
                            // Вернуть значение из бд в контекст
                            throw new DbUpdateException($"Client id {accountTransactionDto.ClientId} " +
                                                        $"has insufficient funds");
                        }

                        foreach (var property in proposedValues.Properties)
                        {
                            var databaseValue = databaseValues[property];
                            proposedValues[property] = databaseValue;
                        }

                        proposedValues["Balance"] = resultBalance;

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                        attemptCount++;
                    }
                    else
                    {
                        throw new NotSupportedException(
                            $"Don't know how to handle concurrency conflicts for {entry.Metadata.Name}");
                    }
                }
            }
            catch (Exception e)
            {
                throw new DbUpdateException($"Failed to debit {accountTransactionDto.Sum} " +
                                            $"to client id {accountTransactionDto.ClientId}: {e.Message}");
            }
        }
        
        if (attemptCount == UpdateMaxAttempts)
        {
            throw new DbUpdateException(
                $"Failed to accrue {accountTransactionDto.Sum} to client id " +
                $"{accountTransactionDto.ClientId} after {UpdateMaxAttempts} attempts");
        }
    }
}
