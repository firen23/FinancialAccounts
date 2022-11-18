using FinancialAccounts.Data;
using FinancialAccounts.Dto;
using FinancialAccounts.Models;
using FinancialAccounts.Utility;
using Microsoft.EntityFrameworkCore;

namespace FinancialAccounts.Services;

public class ClientService : IClientService
{
    private readonly FinancialAccountsContext _context;
    
    public ClientService(FinancialAccountsContext context)
    {
        _context = context;
    }
    
    public async Task<List<ClientDto>> GetClientsDtos()
    {
        var clients =  await _context.Clients.ToListAsync();
        var clientsDtos = clients.Select(ClientDtoConverter.ToClientDto).ToList();
        return clientsDtos;
    }

    public async Task<ClientDto> GetClientDto(long id)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(client => client.Id == id);
        if (client is null)
        {
            throw (new KeyNotFoundException($"Client with id {id} not found"));
        }

        return ClientDtoConverter.ToClientDto(client);
    }

    public async Task AddClient(ClientDto clientDto)
    {
        var client = ClientDtoConverter.ToClient(clientDto);
        await _context.Clients.AddAsync(client);
        var account = new Account { Balance = 0, Client = client };
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateClient(ClientDto clientDto)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(client => client.Id == clientDto.Id);
        if (client is null)
        {
            throw (new KeyNotFoundException($"Client with id {clientDto.Id} not found"));
        }
        
        client.FirstName = clientDto.FirstName;
        client.LastName = clientDto.LastName;
        client.Patronymic = clientDto.Patronymic;
        client.Birthdate = clientDto.Birthdate;
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteClient(long id)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(client => client.Id == id);
        if (client is null)
        {
            throw (new KeyNotFoundException($"Client with id {id} not found"));
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }
}
