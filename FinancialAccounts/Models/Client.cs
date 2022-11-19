namespace FinancialAccounts.Models;

public class Client
{
    public long Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Patronymic { get; set; }
    
    public DateTime Birthdate { get; set; }
}
