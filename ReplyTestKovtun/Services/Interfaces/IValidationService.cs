using ReplyTestKovtun.Models;

namespace ReplyTestKovtun.Services;

public interface IValidationService
{
    void EnsureContractBelongsToUser(Contract contract, Guid userId);
    bool ValidateDynamicFieldValue(FieldType type, string value);
}
