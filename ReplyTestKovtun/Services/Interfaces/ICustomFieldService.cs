using ReplyTestKovtun.DTOs;

namespace ReplyTestKovtun.Services;

public interface ICustomFieldService
{
    Task<List<CustomFieldDto>> GetAll(Guid userId);
    Task Delete(Guid id, Guid userId);
    Task<ContractDto> AssignValue(Guid contractId, AssignCustomFieldValueDto dto, Guid userId);
}
