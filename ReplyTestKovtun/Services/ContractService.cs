using Microsoft.EntityFrameworkCore;
using ReplyTestKovtun.DTOs;
using ReplyTestKovtun.Models;
using ReplyTestKovtun.Repositories;

namespace ReplyTestKovtun.Services;

public class ContractService : IContractService
{
    private readonly IRepository<Contract> _contractRepo;
    private readonly IRepository<ContractDynamicField> _fieldRepo;
    private readonly IValidationService _validationService;

    public ContractService(
        IRepository<Contract> contractRepo,
        IRepository<ContractDynamicField> fieldRepo,
        IValidationService validationService)
    {
        _contractRepo = contractRepo;
        _fieldRepo = fieldRepo;
        _validationService = validationService;
    }

    public async Task<ContractDto> Create(CreateContractDto dto, Guid userId)
    {
        var now = DateTime.UtcNow;
        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            CreatedAt = now,
            ModifiedAt = now,
            UserId = userId
        };
        await _contractRepo.AddAsync(contract);
        await _contractRepo.SaveChangesAsync();
        return MapToDto(contract);
    }

    public async Task<PaginatedResult<ContractDto>> Filter(ContractQueryParams p, Guid userId)
    {
        var query = _contractRepo.Query()
            .Include(c => c.DynamicFields)
            .Where(c => c.UserId == userId);

        if (!string.IsNullOrEmpty(p.FilterName))
            query = query.Where(c => c.Name.Contains(p.FilterName));

        if (!string.IsNullOrEmpty(p.FilterEmail))
            query = query.Where(c => c.Email.Contains(p.FilterEmail));

        query = p.SortBy?.ToLower() switch
        {
            "name" => p.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            "email" => p.SortDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
            _ => p.SortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var items = await query.Skip((p.Page - 1) * p.PageSize).Take(p.PageSize).ToListAsync();

        return new PaginatedResult<ContractDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = p.Page,
            PageSize = p.PageSize
        };
    }

    public async Task<ContractDto> GetById(Guid id, Guid userId)
    {
        var contract = await _contractRepo.Query()
            .Include(c => c.DynamicFields)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId)
            ?? throw new KeyNotFoundException($"Contract {id} not found.");
        return MapToDto(contract);
    }

    public async Task<ContractDto> Update(Guid id, UpdateContractDto dto, Guid userId)
    {
        var contract = await _contractRepo.Query()
            .Include(c => c.DynamicFields)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId)
            ?? throw new KeyNotFoundException($"Contract {id} not found.");

        contract.Name = dto.Name;
        contract.Email = dto.Email;
        contract.Phone = dto.Phone;
        contract.ModifiedAt = DateTime.UtcNow;
        await _contractRepo.SaveChangesAsync();
        return MapToDto(contract);
    }

    public async Task Delete(Guid id, Guid userId)
    {
        var contract = await _contractRepo.Query()
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId)
            ?? throw new KeyNotFoundException($"Contract {id} not found.");
        _contractRepo.Delete(contract);
        await _contractRepo.SaveChangesAsync();
    }

    public async Task BulkMerge(BulkMergeDto dto, Guid userId)
    {
        var existing = await _contractRepo.Query()
            .Include(c => c.DynamicFields)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var existingByEmail = new Dictionary<string, Contract>(StringComparer.OrdinalIgnoreCase);
        foreach (var c in existing)
            existingByEmail.TryAdd(c.Email, c);

        var seenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var deduplicatedItems = dto.Items.Where(i => seenEmails.Add(i.Email)).ToList();

        var toDelete = existing
            .Where(c => !seenEmails.Contains(c.Email))
            .ToList();
        _contractRepo.DeleteRange(toDelete);

        var now = DateTime.UtcNow;
        var newContracts = new List<Contract>();
        var newFields = new List<ContractDynamicField>();

        foreach (var item in deduplicatedItems)
        {
            if (existingByEmail.TryGetValue(item.Email, out var match))
            {
                match.Name = item.Name;
                match.Phone = item.Phone;
                match.ModifiedAt = now;
                _fieldRepo.DeleteRange(match.DynamicFields.ToList());
                if (item.DynamicFields != null)
                    newFields.AddRange(item.DynamicFields.Select(f => new ContractDynamicField
                    {
                        Id = Guid.NewGuid(),
                        ContractId = match.Id,
                        Name = f.Name,
                        Type = f.Type,
                        Value = f.Value
                    }));
            }
            else
            {
                var newContract = new Contract
                {
                    Id = Guid.NewGuid(),
                    Name = item.Name,
                    Email = item.Email,
                    Phone = item.Phone,
                    CreatedAt = now,
                    ModifiedAt = now,
                    UserId = userId
                };
                newContracts.Add(newContract);
                if (item.DynamicFields != null)
                    newFields.AddRange(item.DynamicFields.Select(f => new ContractDynamicField
                    {
                        Id = Guid.NewGuid(),
                        ContractId = newContract.Id,
                        Name = f.Name,
                        Type = f.Type,
                        Value = f.Value
                    }));
            }
        }

        await _contractRepo.AddRangeAsync(newContracts);
        await _fieldRepo.AddRangeAsync(newFields);
        await _contractRepo.SaveChangesAsync();
    }

    private static ContractDto MapToDto(Contract c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone,
        CreatedAt = c.CreatedAt,
        ModifiedAt = c.ModifiedAt,
        DynamicFields = c.DynamicFields?.Select(f => new DynamicFieldDto
        {
            Id = f.Id,
            Name = f.Name,
            Type = f.Type,
            Value = f.Value
        }).ToList() ?? []
    };
}
