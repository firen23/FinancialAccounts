using FinancialAccounts.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccounts.Data;

public class FinancialAccountsContext : DbContext
{
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<AccountTransaction> AccountTransactions { get; set; } = null!;
    
    public FinancialAccountsContext(DbContextOptions<FinancialAccountsContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>().ToTable("Client");
        modelBuilder.Entity<Account>().ToTable(t => 
            t.HasCheckConstraint("Balance", "Balance >= 0"));
        modelBuilder.Entity<AccountTransaction>().ToTable("AccountTransaction");
    }

    public void Reset()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }
}
