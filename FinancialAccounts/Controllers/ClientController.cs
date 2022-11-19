using FinancialAccounts.Dto;
using FinancialAccounts.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialAccounts.Controllers;

[ApiController]
[Route("api/v1/client")]
public class ClientController : ControllerBase
{
    private readonly IClientService _service;
    
    public ClientController(IClientService service)
    {
        _service = service;
    }

    /// <summary>
    /// Gets the list of all <c>ClientDto</c> objects.
    /// </summary>
    /// <returns>List of <c>ClientDto</c> objects.</returns>
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult<IEnumerable<ClientDto>>> Get()
    {
        var clientsDtos = await _service.GetClientsDtos();
        return Ok(clientsDtos);
    }

    /// <summary>
    /// Get ClientDto with client id.
    /// </summary>
    /// <param name="id">Client id</param>
    /// <returns>ClientDto</returns>
    [HttpGet]
    [Route("{id:long}")]
    public async Task<ActionResult<ClientDto>> Get(long id)
    {
        var clientDto = await _service.GetClientDto(id);
        return Ok(clientDto);
    }

    /// <summary>
    /// Add client.
    /// </summary>
    /// <param name="clientDto">ClientDto</param>
    [HttpPost]
    [Route("add")]
    public async Task<ActionResult> Post(ClientDto clientDto)
    {
        await _service.AddClient(clientDto);
        return Ok();
    }

    /// <summary>
    /// Update client.
    /// </summary>
    /// <param name="clientDto">ClientDto</param>
    [HttpPut]
    [Route("update")]
    public async Task<ActionResult> Put(ClientDto clientDto)
    {
        await _service.UpdateClient(clientDto);
        return Ok();
    }

    /// <summary>
    /// Delete client.
    /// </summary>
    /// <param name="id">Client id</param>
    [HttpDelete]
    [Route("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        await _service.DeleteClient(id);
        return Ok();
    }
}
