using FinancialAccounts.Dto;

namespace FinancialAccounts.Services;

public interface IClientService
{
    public Task<List<ClientDto>> GetClientsDtos();

    public Task<ClientDto> GetClientDto(long id);

    public Task AddClient(ClientDto clientDto);

    public Task UpdateClient(ClientDto clientDto);

    public Task DeleteClient(long id);
}
