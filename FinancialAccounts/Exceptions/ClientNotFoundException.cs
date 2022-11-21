namespace FinancialAccounts.Exceptions;

public class ClientNotFoundException : KeyNotFoundException
{
    public ClientNotFoundException(long clientId) :
        base($"Client id {clientId} not found")
    { }
}
