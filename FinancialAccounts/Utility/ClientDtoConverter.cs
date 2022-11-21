using FinancialAccounts.Dto;
using FinancialAccounts.Models;

namespace FinancialAccounts.Utility;

public static class ClientDtoConverter
{
    public static Client ToClient(ClientDto clientDto)
    {
        return new Client
        {
            FirstName = clientDto.FirstName,
            LastName = clientDto.LastName,
            Patronymic = clientDto.Patronymic,
            Birthdate = clientDto.Birthdate
        };
    }

    public static ClientDto ToClientDto(Client client)
    {
        return new ClientDto
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Patronymic = client.Patronymic,
            Birthdate = client.Birthdate
        };
    }

    public static void UpdateFromClientDto(Client client, ClientDto clientDto)
    {
        client.FirstName = clientDto.FirstName;
        client.LastName = clientDto.LastName;
        client.Patronymic = clientDto.Patronymic;
        client.Birthdate = clientDto.Birthdate;
    }
}
