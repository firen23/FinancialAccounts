using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Test;

public class AccountTransactions
{
    public long ClientId { get; set; }
    
    public List<Transaction> Transactions { get; set; }

    public decimal GetSum()
    {
        return Transactions.Where(transaction => transaction.IsAccepted).Sum(transaction => transaction.Sum);
    }

    public AccountTransactions(long clientId)
    {
        ClientId = clientId;
        Transactions = new List<Transaction>();
    }
}

public class Transaction
{
    public decimal Sum { get; set; }
    
    public bool IsAccepted { get; set; }
    
    public string ErrorMessage { get; set; }
}

public class AccountSum
{
    public long ClientId { get; set; }
    
    public decimal Sum { get; set; }
}
