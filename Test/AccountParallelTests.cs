using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using FinancialAccounts.Data;
using FinancialAccounts.Dto;
using FinancialAccounts.Services;
using Test.Utility;

namespace Test;

public class AccountParallelTests
{
    private const float MinSum = 0.01f;
    private const float MaxSum = 1000f;
    private const int OperationsInThread = 100;
    private const int ThreadsCount = 5;

    private DbContextOptions<FinancialAccountsContext> _options; 
    
    [SetUp]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
        IConfiguration config = builder.Build();
        
        _options = new DbContextOptionsBuilder<FinancialAccountsContext>()
            .UseSqlServer(config.GetConnectionString("TestConnection"))
            .Options;

        using (var context = new FinancialAccountsContext(_options))
        {
            context.Reset();
            AddTestClientsToDbContext(context);
        }
    }
    
    [Test]
    public void TestParallelTransactions()
    {
        var threadsAccounts = new List<List<AccountTransactions>>();
        var threads = new List<Thread>();
        
        for (var i = 0; i < ThreadsCount; i++)
        {
            var baseAccounts = GetListOfAccounts(new FinancialAccountsContext(_options));
            threadsAccounts.Add(new List<AccountTransactions>(baseAccounts));
            threads.Add(new Thread(() => 
                DoRandomTransactions(new AccountService(new FinancialAccountsContext(_options)),
                    new List<AccountTransactions>(baseAccounts))));
        }

        threads.ForEach(thread => thread.Start());
        threads.ForEach(thread => thread.Join());

        var totalTransactionsCount = threadsAccounts
            .SelectMany(list => list)
            .Sum(x => x.Transactions.Count);

        Assert.True(totalTransactionsCount == ThreadsCount * OperationsInThread);

        var listOfAllErrors = threadsAccounts
                .SelectMany(list => list)
                .Select(account => account.Transactions)
                .SelectMany(transactions => transactions)
                .Where(transaction => transaction.IsAccepted is false)
            ;

        var listOfResults = threadsAccounts
            .SelectMany(list => list)
            .GroupBy(account => account.ClientId)
            .Select(group => new AccountSum
            {
                ClientId = group.Key,
                Sum = group.Sum(account => account.GetSum())
            })
            .ToList();

        var listOfAccountBalance = GetListOfAccountSumsFromDb(new FinancialAccountsContext(_options));

        for (var i = 0; i < listOfAccountBalance.Count; i++)
        {
            if (!listOfResults[i].Sum.Equals(listOfAccountBalance[i].Sum))
            {
                Assert.Fail();
            }
        }

        Assert.Pass();
    }
    
    private void DoRandomTransactions(IAccountService service,
        List<AccountTransactions> accounts)
    {
        var random = ThreadLocalRandom.Instance;
        for (var i = 0; i < OperationsInThread; i++)
        {
            var randomAccount = random.Next(accounts.Count);
            var randomMethod = random.Next(0, 2);
            var randomSum = random.NextDouble() * (MaxSum - MinSum) + MinSum;
            var decimalSum = Convert.ToDecimal(Math.Round(randomSum, 2));

            var transaction = new Transaction
            {
                Sum = randomMethod == 1 ? decimalSum : (-1 * decimalSum)
            };
            
            var accountTransactionDto = new AccountTransactionDto
            {
                ClientId = accounts[randomAccount].ClientId,
                Sum = decimalSum
            };
            
            try
            {
                
                switch (randomMethod)
                {
                    case 0:
                    {
                        service.Debit(accountTransactionDto).Wait();
                        break;
                    }
                    case 1:
                    {
                        service.Accrue(accountTransactionDto).Wait();
                        break;
                    }
                }

                transaction.IsAccepted = true;
            }
            catch (Exception e)
            {
                transaction.IsAccepted = false;
                transaction.ErrorMessage = e.Message;
            }
            
            accounts[randomAccount].Transactions.Add(transaction);
        }

        var count = accounts.Count;
    }

    private List<AccountTransactions> GetListOfAccounts(FinancialAccountsContext context)
    {
        return context.Clients
            .Select(client => new AccountTransactions(client.Id))
            .AsNoTracking()
            .ToList();
    }

    private List<AccountSum> GetListOfAccountSumsFromDb(FinancialAccountsContext context)
    {
        return context.Accounts
            .Select(account => new AccountSum
            {
                ClientId = account.ClientId,
                Sum = account.Balance
            }).AsNoTracking()
            .ToList();
    }

    private void AddTestClientsToDbContext(FinancialAccountsContext context)
    {
        var data = new TestData();
        context.Clients.AddRange(data.Clients);
        context.Accounts.AddRange(data.Accounts);
        context.SaveChanges();
    }
}
