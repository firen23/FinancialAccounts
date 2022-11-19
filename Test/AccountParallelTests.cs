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
using FinancialAccounts.Models;
using FinancialAccounts.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Test.Utility;

namespace Test;

public class AccountParallelTests
{
    private const float MinSum = 0.01f;
    private const float MaxSum = 1000f;
    private const int OperationsInThread = 100;
    private const int ThreadsCount = 10;

    private IDbContextFactory<FinancialAccountsContext> _contextFactory;

    [SetUp]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
        IConfiguration config = builder.Build();

        var services = new ServiceCollection();
        services.AddPooledDbContextFactory<FinancialAccountsContext>(options => 
            options.UseSqlServer(config.GetConnectionString("TestConnection")));
        var provider = services.BuildServiceProvider();
        _contextFactory = provider.GetRequiredService<IDbContextFactory<FinancialAccountsContext>>();

        AddTestClientsToDbContext();
    }

    [Test]
    public void TestParallelTransactions()
    {
        var threadsAccounts = new List<List<StoredTransactions>>();
        var threads = new List<Thread>();
        
        for (var i = 0; i < ThreadsCount; i++)
        {
            var baseAccounts = GetListOfAccounts();
            threadsAccounts.Add(new List<StoredTransactions>(baseAccounts));
            threads.Add(new Thread(() => 
                DoRandomTransactions(new AccountService(_contextFactory),
                    new List<StoredTransactions>(baseAccounts))));
        }

        threads.ForEach(thread => thread.Start());
        threads.ForEach(thread => thread.Join());

        var totalTransactionsCount = threadsAccounts
            .SelectMany(list => list)
            .Sum(x => x.Transactions.Count);

        Assert.True(totalTransactionsCount == ThreadsCount * OperationsInThread);
        
        var listOfResults = threadsAccounts
            .SelectMany(list => list)
            .GroupBy(account => account.ClientId)
            .Select(group => new AccountSum
            {
                ClientId = group.Key,
                Sum = group.Sum(account => account.GetSum())
            })
            .ToList();

        var listOfAccountBalance = GetListOfAccountSumsFromDb();

        for (var i = 0; i < listOfAccountBalance.Count; i++)
        {
            if (!listOfResults[i].Sum.Equals(listOfAccountBalance[i].Sum))
            {
                Assert.Fail();
            }
        }

        Assert.Pass();
    }
    
    private void DoRandomTransactions(IAccountService service, List<StoredTransactions> accounts)
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

                transaction.State = TransactionState.Accepted;
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions is DbUpdateException)
                {
                    transaction.State = TransactionState.Rejected;
                    transaction.ErrorMessage = e.Message;                    
                }
            }
            
            accounts[randomAccount].Transactions.Add(transaction);
        }
    }

    private List<StoredTransactions> GetListOfAccounts()
    {
        List<StoredTransactions> list;
        using (var context = _contextFactory.CreateDbContext())
        {
            list = context.Clients
                .Select(client => new StoredTransactions(client.Id))
                .AsNoTracking()
                .ToList();            
        }

        return list;
    }

    private List<AccountSum> GetListOfAccountSumsFromDb()
    {
        List<AccountSum> list;
        using (var context = _contextFactory.CreateDbContext())
        {
            list = context.Accounts
                .Select(account => new AccountSum
                {
                    ClientId = account.ClientId,
                    Sum = account.Balance
                }).AsNoTracking()
                .ToList();
        }

        return list;
    }

    private void AddTestClientsToDbContext()
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            context.Reset();
            
            var data = new TestData();
            context.Clients.AddRange(data.Clients);
            context.Accounts.AddRange(data.Accounts);
            context.SaveChanges();
        }
    }
}
