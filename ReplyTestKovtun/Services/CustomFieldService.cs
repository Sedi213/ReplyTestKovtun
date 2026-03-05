using Microsoft.EntityFrameworkCore;
using ReplyTestKovtun.DTOs;
using ReplyTestKovtun.Models;
using ReplyTestKovtun.Repositories;

namespace ReplyTestKovtun.Services;

public class CustomFieldService : ICustomFieldService
{
    private readonly IRepository<ContractDynamicField> _fieldRepo;
    private readonly IRepository<Contract> _contractRepo;
    private readonly IValidationService _validationService;

    public CustomFieldService(
        IRepository<ContractDynamicField> fieldRepo,
        IRepository<Contract> contractRepo,
        IValidationService validationService)
    {
        _fieldRepo = fieldRepo;
        _contractRepo = contractRepo;
        _validationService = validationService;
    }

    public async Task<List<CustomFieldDto>> GetAll(Guid userId)
    {
        var fields = await _fieldRepo.Query()
            .Include(f => f.Contract)
            .Where(f => f.Contract.UserId == userId)
            .Select(f => new { f.Name, f.Type })
            .Distinct()
            .ToListAsync();

        return fields.Select(f => new CustomFieldDto { Name = f.Name, Type = f.Type }).ToList();
    }

    public async Task Delete(Guid id, Guid userId)
    {
        var field = await _fieldRepo.Query()
            .Include(f => f.Contract)
            .FirstOrDefaultAsync(f => f.Id == id)
            ?? throw new KeyNotFoundException($"Field {id} not found.");

        _validationService.EnsureContractBelongsToUser(field.Contract, userId);

        _fieldRepo.Delete(field);
        await _fieldRepo.SaveChangesAsync();
    }

    public async Task<ContractDto> AssignValue(Guid contractId, AssignCustomFieldValueDto dto, Guid userId)
    {
        var contract = await _contractRepo.Query()
            .Include(c => c.DynamicFields)
            .FirstOrDefaultAsync(c => c.Id == contractId)
            ?? throw new KeyNotFoundException($"Contract {contractId} not found.");

        _validationService.EnsureContractBelongsToUser(contract, userId);

        if (!_validationService.ValidateDynamicFieldValue(dto.Type, dto.Value))
            throw new ArgumentException($"Value '{dto.Value}' is not valid for type {dto.Type}.");

        var existing = contract.DynamicFields.FirstOrDefault(f => f.Name == dto.Name);
        if (existing != null)
        {
            existing.Type = dto.Type;
            existing.Value = dto.Value;
        }
        else
        {
            await _fieldRepo.AddAsync(new ContractDynamicField
            {
                Id = Guid.NewGuid(),
                ContractId = contractId,
                Name = dto.Name,
                Type = dto.Type,
                Value = dto.Value
            });
        }

        contract.ModifiedAt = DateTime.UtcNow;
        await _fieldRepo.SaveChangesAsync();

        return new ContractDto
        {
            Id = contract.Id,
            Name = contract.Name,
            Email = contract.Email,
            Phone = contract.Phone,
            CreatedAt = contract.CreatedAt,
            ModifiedAt = contract.ModifiedAt,
            DynamicFields = contract.DynamicFields.Select(f => new DynamicFieldDto
            {
                Id = f.Id,
                Name = f.Name,
                Type = f.Type,
                Value = f.Value
            }).ToList()
        };
    }
}
