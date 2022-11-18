using FinancialAccounts.Dto;
using FinancialAccounts.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccounts.Controllers;

[ApiController]
[Route("api/v1/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _service;

    public AccountController(IAccountService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get current balance of client with id.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns>ClientBalanceDto</returns>
    [HttpGet]
    [Route("{clientId:long}")]
    public async Task<ActionResult<ClientBalanceDto>> GetBalance(long clientId)
    {
        var clientBalanceDto = await _service.GetBalance(clientId);
        return Ok(clientBalanceDto);
    }

    /// <summary>
    /// Accrue sum from client with id.
    /// </summary>
    /// <param name="accountTransactionDto">AccountTransactionDto</param>
    [HttpPost]
    [Route("accrue")]
    public async Task<ActionResult> Accrue(AccountTransactionDto accountTransactionDto)
    {
        await _service.Accrue(accountTransactionDto);
        return Ok();
    }

    /// <summary>
    /// Debit sum from client with id.
    /// </summary>
    /// <param name="accountTransactionDto">AccountTransactionDto</param>
    [HttpPost]
    [Route("debit")]
    public async Task<ActionResult> Debit(AccountTransactionDto accountTransactionDto)
    {
        await _service.Debit(accountTransactionDto);
        return Ok();
    }
}
