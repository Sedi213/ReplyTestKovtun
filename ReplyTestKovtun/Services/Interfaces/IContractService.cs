using ReplyTestKovtun.DTOs;

namespace ReplyTestKovtun.Services;

public interface IContractService
{
    Task<ContractDto> Create(CreateContractDto dto, Guid userId);
    Task<PaginatedResult<ContractDto>> Filter(ContractQueryParams queryParams, Guid userId);
    Task<ContractDto> GetById(Guid id, Guid userId);
    Task<ContractDto> Update(Guid id, UpdateContractDto dto, Guid userId);
    Task Delete(Guid id, Guid userId);
    Task BulkMerge(BulkMergeDto dto, Guid userId);
}
