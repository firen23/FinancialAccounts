using System.Collections.Generic;
using System.Linq;
using FinancialAccounts.Models;

namespace Test;

public class StoredTransactions
{
    public long ClientId { get; set; }
    
    public List<Transaction> Transactions { get; set; }

    public decimal GetSum()
    {
        return Transactions
            .Where(transaction => transaction.State == TransactionState.Accepted)
            .Sum(transaction => transaction.Sum);
    }

    public StoredTransactions()
    {
        
    }

    public StoredTransactions(long clientId)
    {
        ClientId = clientId;
        Transactions = new List<Transaction>();
    }
}

public class Transaction
{
    public decimal Sum { get; set; }
    
    public TransactionState State { get; set; }
    
    public string ErrorMessage { get; set; }
}

public class AccountSum
{
    public long ClientId { get; set; }
    
    public decimal Sum { get; set; }
}
