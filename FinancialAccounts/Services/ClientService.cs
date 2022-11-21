using FinancialAccounts.Data;
using FinancialAccounts.Dto;
using FinancialAccounts.Exceptions;
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
    
    public async Task<List<ClientDto>> GetClientsDto()
    {
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var clients =  await context.Clients.ToListAsync();
            var clientsDtos = clients
                .Select(ClientDtoConverter.ToClientDto)
                .ToList();
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
                throw new ClientNotFoundException(id);
            }

            clientDto = ClientDtoConverter.ToClientDto(client);
        }

        return clientDto;
    }

    public async Task AddClientDto(ClientDto clientDto)
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

    public async Task UpdateClientDto(ClientDto clientDto)
    {
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            var client = await context.Clients
                .FirstOrDefaultAsync(client => client.Id == clientDto.Id);
            if (client is null)
            {
                throw new ClientNotFoundException(clientDto.Id.Value);
            }
            
            ClientDtoConverter.UpdateFromClientDto(client, clientDto);
            context.Clients.Update(client);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteClient(long id)
    {
        using (var context = await _contextFactory.CreateDbContextAsync())
        {
            try
            {
                context.Clients.Remove(new Client {Id = id});
                await context.SaveChangesAsync();
            }
            catch
            {
                if (!context.Clients.Any(client => client.Id == id))
                {
                    throw new ClientNotFoundException(id);
                }
                else
                {
                    throw;
                }
            }
            
        }
    }
}
