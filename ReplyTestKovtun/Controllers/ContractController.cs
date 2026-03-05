using Microsoft.AspNetCore.Mvc;
using ReplyTestKovtun.DTOs;
using ReplyTestKovtun.Services;

namespace ReplyTestKovtun.Controllers;

[ApiController]
[Route("api/contracts")]
public class ContractController : ControllerBase
{
    private readonly IContractService _contractService;
    private readonly ICustomFieldService _customFieldService;
    private readonly IContextService _contextService;

    public ContractController(IContractService contractService, ICustomFieldService customFieldService, IContextService contextService)
    {
        _contractService = contractService;
        _customFieldService = customFieldService;
        _contextService = contextService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContractDto dto)
    {
        var result = await _contractService.Create(dto, _contextService.UserId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("filter")]
    public async Task<IActionResult> Filter([FromBody] ContractQueryParams queryParams)
    {
        var result = await _contractService.Filter(queryParams, _contextService.UserId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _contractService.GetById(id, _contextService.UserId);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContractDto dto)
    {
        var result = await _contractService.Update(id, dto, _contextService.UserId);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _contractService.Delete(id, _contextService.UserId);
        return NoContent();
    }

    [HttpPost("bulk-merge")]
    public async Task<IActionResult> BulkMerge([FromBody] BulkMergeDto dto)
    {
        await _contractService.BulkMerge(dto, _contextService.UserId);
        return Ok();
    }
}
