using FinancialAccounts.Data;
using FinancialAccounts.Dto;
using FinancialAccounts.Models;
using FinancialAccounts.Utility;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccounts.Services;

public class ClientService : IClientService
{
    private readonly IDbContextFactory<FinancialAccountsContext> _contextFactory;
    
    public ClientService(IDbContextFactory<FinancialAccountsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<List<ClientDto>> GetClientsDtos()
    {
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var clients =  await context.Clients.ToListAsync();
            var clientsDtos = clients.Select(ClientDtoConverter.ToClientDto).ToList();
            return clientsDtos;            
        }
    }

    public async Task<ClientDto> GetClientDto(long id)
    {
        ClientDto clientDto;
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var client = await context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(client => client.Id == id);
            if (client is null)
            {
                throw (new KeyNotFoundException($"Client with id {id} not found"));
            }

            clientDto = ClientDtoConverter.ToClientDto(client);
        }

        return clientDto;
    }

    public async Task AddClient(ClientDto clientDto)
    {
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var client = ClientDtoConverter.ToClient(clientDto);
            await context.Clients.AddAsync(client);
            var account = new Account {Balance = 0, Client = client};
            await context.Accounts.AddAsync(account);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateClient(ClientDto clientDto)
    {
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var client = await context.Clients.FirstOrDefaultAsync(client => client.Id == clientDto.Id);
            if (client is null)
            {
                throw (new KeyNotFoundException($"Client with id {clientDto.Id} not found"));
            }

            client.FirstName = clientDto.FirstName;
            client.LastName = clientDto.LastName;
            client.Patronymic = clientDto.Patronymic;
            client.Birthdate = clientDto.Birthdate;
            context.Clients.Update(client);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteClient(long id)
    {
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var client = await context.Clients.FirstOrDefaultAsync(client => client.Id == id);
            if (client is null)
            {
                throw (new KeyNotFoundException($"Client with id {id} not found"));
            }

            context.Clients.Remove(client);
            await context.SaveChangesAsync();
        }
    }
}
