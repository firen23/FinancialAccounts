using FinancialAccounts.Dto;

namespace FinancialAccounts.Models;

public class Client
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Patronymic { get; set; }
    public DateTime Birthdate { get; set; }
    
    // public Account? Account { get; set; }

    // public Client(ClientDto clientDto)
    // {
    //     FirstName = clientDto.FirstName;
    //     LastName = clientDto.LastName;
    //     Patronymic = clientDto.Patronymic;
    //     Birthdate = clientDto.Birthdate;
    // }
}
