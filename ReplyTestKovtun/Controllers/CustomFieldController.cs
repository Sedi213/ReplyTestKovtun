using Microsoft.AspNetCore.Mvc;
using ReplyTestKovtun.DTOs;
using ReplyTestKovtun.Services;

namespace ReplyTestKovtun.Controllers;

[ApiController]
[Route("api/custom-fields")]
public class CustomFieldController : ControllerBase
{
    private readonly ICustomFieldService _customFieldService;
    private readonly IContextService _contextService;

    public CustomFieldController(ICustomFieldService customFieldService, IContextService contextService)
    {
        _customFieldService = customFieldService;
        _contextService = contextService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _customFieldService.GetAll(_contextService.UserId);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        await _customFieldService.Delete(id, _contextService.UserId);
        return NoContent();
    }


    [HttpPost("{contractId:guid}/fields")]
    public async Task<IActionResult> AssignCustomFieldValue(Guid contractId, [FromBody] AssignCustomFieldValueDto dto)
    {
        var result = await _customFieldService.AssignValue(contractId, dto, _contextService.UserId);
        return Ok(result);
    }
}
