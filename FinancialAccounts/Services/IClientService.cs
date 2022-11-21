using FinancialAccounts.Dto;

namespace FinancialAccounts.Services;

public interface IClientService
{
    public Task<List<ClientDto>> GetClientsDto();

    public Task<ClientDto> GetClientDto(long id);

    public Task AddClientDto(ClientDto clientDto);

    public Task UpdateClientDto(ClientDto clientDto);

    public Task DeleteClient(long id);
}
